using Discord.Bots.Core.Services;
using Discord.Interactions;
using Discord.WebSocket;

namespace Discord.Bots.Core.Modules;

public class ButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly ConfigService _configService;

    public ButtonModule(ConfigService configService)
    {
        _configService = configService;
    }

    [ComponentInteraction("quiz_*_*")]
    public async Task Quiz(string correct, string answer)
    {
        if (!int.TryParse(correct, out int correctInt))
        {

            await RespondAsync("Something went wrong with the answer button, please contact one of the wardens", ephemeral: true);
            return;
        }

        bool correctBool = Convert.ToBoolean(correctInt);
        answer = answer.Replace("|", " ");
        string text = correctBool
            ? $"`{answer}` is correct! Congratulations {Context.Interaction.User.Mention}"
            : $"`{answer}` was not the correct answer, {Context.Interaction.User.Mention}";

        await Context.Interaction.UpdateAsync(x =>
        {
            x.Content = text;
            x.Components = null;
            x.Embeds = null;
        });
    }
}
