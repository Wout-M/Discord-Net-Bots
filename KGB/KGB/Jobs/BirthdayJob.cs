using Discord;
using Discord.WebSocket;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using KGB.Services;

namespace KGB.Jobs
{
    [DisallowConcurrentExecution]
    public class BirthdayJob : IJob
    {
        private readonly ConfigService _configService;
        private readonly DiscordSocketClient _client;

        public BirthdayJob(ConfigService configService, DiscordSocketClient client)
        {
            _configService = configService;
            _client = client;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var config = await _configService.GetConfig(true);

            foreach (var server in config.Servers)
            {
                var serverId = server.Key;
                var serverConfig = server.Value;

                if (!serverConfig.EnableBirthdayChecking || !serverConfig.Birthdays.Any())
                {
                    continue;
                }

                var today = DateTime.Today;
                var birthdaysAndDifferences = serverConfig.Birthdays
                    .Select(birthday =>
                    {
                        var difference = new DateTime(today.Year, birthday.birthday.Month, birthday.birthday.Day) - today;
                        return (birthday, difference);
                    })
                    .Where(x => x.difference <= TimeSpan.FromDays(1) && x.difference > TimeSpan.Zero);

                if (birthdaysAndDifferences.Any())
                {
                    var guild = _client.GetGuild(serverId);
                    var events = await guild.GetEventsAsync();

                    foreach (var birthdayAndDifference in birthdaysAndDifferences)
                    {
                        var birthday = birthdayAndDifference.birthday;
                        var difference = birthdayAndDifference.difference;
                        var user = await _client.GetUserAsync(birthday.userId);

                        if (!events.Any(x => x.Description.Contains(user.Username)))
                        {
                            try
                            {
                                await guild.CreateEventAsync(name: $"{user.Username}'s birthday",
                                                             startTime: new DateTimeOffset(today.AddTicks(difference.Ticks)),
                                                             type: GuildScheduledEventType.External,
                                                             description: $"It's {user.Username}'s birthday. They'll turn {today.Year - birthday.birthday.Year} years old.",
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
        }
    }
}
