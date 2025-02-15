using Discord.Bots.Core.Services;
using Discord.WebSocket;
using Quartz;

namespace Discord.Bots.Core.Jobs;

[DisallowConcurrentExecution]
public class BirthdayJob(ConfigService configService, DiscordSocketClient client) : IJob
{
    private readonly ConfigService _configService = configService;
    private readonly DiscordSocketClient _client = client;

    public async Task Execute(IJobExecutionContext context)
    {
        var config = await _configService.GetConfig(true);

        foreach (var server in config.Servers)
        {
            var serverId = server.Key;
            var serverConfig = server.Value;

            if (!serverConfig.EnableBirthdayChecking || !serverConfig.Birthdays.Any()) continue;

            var today = DateTime.Today;
            var birthdaysAndDifferences = serverConfig.Birthdays
                .Select(birthday =>
                {
                    var difference = new DateTime(today.Year, birthday.birthday.Month, birthday.birthday.Day) - today;
                    return (birthday, difference);
                })
                .Where(x => x.difference <= TimeSpan.FromDays(1) && x.difference > TimeSpan.Zero);

            if (!birthdaysAndDifferences.Any()) continue;

            var guild = _client.GetGuild(serverId);
            var events = await guild.GetEventsAsync();

            foreach (var (birthday, difference) in birthdaysAndDifferences)
            {
                var user = await _client.GetUserAsync(birthday.userId);
                if (events.Any(x => x.Description.Contains(user.Mention))) continue;

                try
                {
                    var guildUser = await (guild as IGuild).GetUserAsync(birthday.userId);
                    var ageMessage = $" They'll turn {today.Year - birthday.birthday.Year} years old.";
                    await guild.CreateEventAsync(name: $"{guildUser.DisplayName}'s birthday",
                                                 startTime: new DateTimeOffset(today.AddTicks(difference.Ticks)),
                                                 type: GuildScheduledEventType.External,
                                                 description: $"It's {user.Mention}'s birthday.{(Config.Config.ShowAge ? ageMessage : string.Empty)}",
                                                 endTime: new DateTimeOffset(today.AddDays(1).AddTicks(difference.Ticks)),
                                                 location: "Here");
                }
                catch (Exception ex)
                {
                    if (serverConfig.ModChannel.HasValue)
                    {
                        var channel = await _client.GetChannelAsync(serverConfig.ModChannel.Value);
                        if (channel is ITextChannel textChannel)
                        {
                            await textChannel.SendMessageAsync(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
