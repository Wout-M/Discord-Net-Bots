using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using KGB.Services;

namespace KGB.Modules
{
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

        [ComponentInteraction("words-*")]
        public async Task Words(string word)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            var scores = config.Words.First(w => w.word == word).scores.OrderBy(x => x.count).Reverse();

            var scoresEmbed = new EmbedBuilder()
                .WithTitle($"How much has `{word}` been said in {Context.Guild.Name}")
                .WithDescription($"`{word}` has been said `{scores.Sum(x => x.count)}` times")
                .WithFooter("Created by the almighty ginger");

            if (scores.Any())
            {
                var usersText = string.Join(Environment.NewLine, scores.Select(s => Context.Guild.GetUser(s.userId).DisplayName));
                var scoresText = string.Join(Environment.NewLine, scores.Select(s => s.count));
                scoresEmbed.AddField("Names", usersText, true)
                           .AddField("Scores", scoresText, true);
            }
            else
            {
                scoresEmbed.AddField("Scores", "This word hasn't been said yet");
            }

            await Context.Interaction.UpdateAsync(x =>
            {
                x.Content = null;
                x.Components = null;
                x.Embed = scoresEmbed.Build();
            });
        }
    }
}
