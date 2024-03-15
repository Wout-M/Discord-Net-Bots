using Discord.Bots.Core.Services;
using Discord.Interactions;

namespace Discord.Bots.Core.Modules;

[Group("words", "Counting words")]
public class WordsModule : InteractionModuleBase<InteractionContext>
{
    private readonly ConfigService _configService;

    public WordsModule(ConfigService configService)
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
    public async Task Menu([Summary("word", "See how much everyone says a word")] string word)
    {
        var config = await _configService.GetServerConfig(Context.Guild.Id);

        word = word.ToLower();

        if (!config.Words.Any(x => x.word == word))
        {
            await RespondAsync("This word is not being counted");
        }
        else
        {
            await DeferAsync();

            var scores = config.Words.First(w => w.word == word).scores.OrderBy(x => x.count).Reverse();
            var scoresEmbed = new EmbedBuilder()
                .WithTitle($"How much has `{word}` been said in {Context.Guild.Name}")
                .WithDescription($"`{word}` has been said `{scores.Sum(x => x.count)}` times")
                .WithFooter("Created by the almighty ginger");

            if (scores.Any())
            {
                var users = await Task.WhenAll(scores.Select(async s =>
                {
                    var username = string.Empty;
                    var guildUser = await (Context.Guild as IGuild).GetUserAsync(s.userId);
                    if (guildUser != null)
                    {
                        username = string.IsNullOrEmpty(guildUser.Nickname) ? guildUser.Username : guildUser.Nickname;
                    }
                    else
                    {
                        var user = await Context.Client.GetUserAsync(s.userId);
                        username = string.IsNullOrEmpty(user?.Username) ? "User not found" : user.Username;
                    }

                    return username;
                }));

                var usersText = string.Join(Environment.NewLine, users);
                var scoresText = string.Join(Environment.NewLine, scores.Select(s => s.count));
                scoresEmbed.AddField("Names", usersText, true)
                           .AddField("Scores", scoresText, true);
            }
            else
            {
                scoresEmbed.AddField("Scores", "This word hasn't been said yet");
            }

            await ModifyOriginalResponseAsync(x =>
            {
                x.Content = null;
                x.Components = null;
                x.Embed = scoresEmbed.Build();
            });
        }
    }
}
