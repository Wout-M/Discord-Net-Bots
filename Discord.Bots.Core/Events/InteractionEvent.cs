using Discord.Interactions;
using Discord.WebSocket;

namespace Discord.Bots.Core.Events;

public class InteractionEvent(IServiceProvider provider, DiscordSocketClient client, InteractionService commands)
{
    private readonly IServiceProvider _provider = provider;
    private readonly DiscordSocketClient _client = client;
    private readonly InteractionService _commands = commands;

    public static async Task SlashCommandExecuted(SlashCommandInfo command, IInteractionContext context, IResult result)
    {
        if (result.Error.HasValue)
        {
            switch (result.Error.Value)
            {
                //case CommandError.UnknownCommand:
                //    break;
                //case CommandError.ParseFailed:
                //    break;
                //case CommandError.BadArgCount:

                //    break;
                //case CommandError.ObjectNotFound:
                //    break;
                //case CommandError.MultipleMatches:
                //    break;
                case InteractionCommandError.UnmetPrecondition:
                    await context.Interaction.RespondAsync($"{context.User.Mention}, you don't have permissions to use this command");
                    break;
                case InteractionCommandError.Exception:
                    await context.Interaction.RespondAsync($"{context.User.Mention}, an error occured while using this command");
                    break;
                    //case CommandError.Unsuccessful:
                    //    break;
                    //default:
                    //    break;
            }
        }
    }

    public async Task HandleInteraction(SocketSlashCommand interaction)
    {
        try
        {
            // create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
            var ctx = new InteractionContext(_client, interaction);
            await _commands.ExecuteCommandAsync(ctx, _provider);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            // if a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }

    public async Task HandleButtonInteraction(SocketMessageComponent interaction)
    {
        var ctx = new SocketInteractionContext<SocketMessageComponent>(_client, interaction);
        await _commands.ExecuteCommandAsync(ctx, _provider);
    }
}
