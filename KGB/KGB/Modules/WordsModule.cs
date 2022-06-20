using Discord;
using Discord.Interactions;
using KGB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGB.Modules
{
    [Group("words", "Counting words")]
    public class SortModule : InteractionModuleBase<InteractionContext>
    {
        private readonly ConfigService _configService;

        public SortModule(ConfigService configService)
        {
            _configService = configService;
        }

        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [SlashCommand("add", "Add a new word")]
        public async Task Add([Summary("word", "A word that should be counted")] string word)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            word = word.Trim().ToLower();

            if (!config.Words.Any(x => x.word == word))
            {
                config.Words.Add((word, new List<(ulong userId, int count)>()));
                await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
                await RespondAsync($"Successfully added `{word}` to the words list");
            }
            else
            {
                await RespondAsync($"`{word}` is already in the words list");
            }
        }

        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [SlashCommand("remove", "Remove a word")]
        public async Task Remove([Summary("word", "A word that should be removed from the list")] string word)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            word = word.Trim().ToLower();

            if (!config.Words.Any(x => x.word == word))
            {
                await RespondAsync($"`{word}` is not in the list");
            }
            else
            {
                var wordItem = config.Words.First(x => x.word == word);
                config.Words.Remove(wordItem);
                await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
                await RespondAsync($"Successfully removed `{word}` from the list");

            }
        }

        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [SlashCommand("list", "List the words that are counted")]
        public async Task List()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);

            string text = config.Words.Any()
                ? string.Join("\n", config.Words.Select(w => $"- {w.word}"))
                : "No words";

            var embedBuilder = new EmbedBuilder()
               .WithDescription("Here are the words that are counted")
               .WithColor(Color.DarkGreen)
               .AddField("Words", text);

            await RespondAsync(embed: embedBuilder.Build());
        }

        [SlashCommand("score", "Display the scoreboard")]
        public async Task Menu()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);

            if (!config.Words.Any())
            {
                await RespondAsync("No words configured");
            }
            else
            {
                var embedBuilder = new EmbedBuilder()
                    .WithTitle("Choose the word you want to see the scoreboard of")
                    .WithDescription("Click on the button with the word")
                    .WithColor(Color.Purple)
                    .WithFooter("Created by the almighty Ginger");

                var buttonBuilder = new ComponentBuilder();

                for (int i = 0; i < config.Words.Count; i++)
                {
                    var button = new ButtonBuilder()
                    {
                        Label = config.Words[i].word,
                        CustomId = $"words-{config.Words[i].word}",
                        Style = (ButtonStyle)(((i + 1) % 4) + 1)
                    };
                    buttonBuilder.WithButton(button);
                }

                await RespondAsync(embed: embedBuilder.Build(), components: buttonBuilder.Build());
            }
        }
    }
}
