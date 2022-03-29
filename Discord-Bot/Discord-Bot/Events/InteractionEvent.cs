using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord_Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Events
{
    public class InteractionEvent
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly ConfigService _configService;
        private readonly IServiceProvider _provider;

        public InteractionEvent(
            DiscordSocketClient client,
            InteractionService commands,
            ConfigService configService,
            IServiceProvider provider)
        {
            _client = client;
            _commands = commands;
            _configService = configService;
            _provider = provider;

            _commands.SlashCommandExecuted += SlashCommandExecuted;

        }

        public async Task SlashCommandExecuted(SlashCommandInfo command, IInteractionContext context, IResult result)
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
}
