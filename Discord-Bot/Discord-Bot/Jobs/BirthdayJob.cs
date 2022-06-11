using Discord;
using Discord.WebSocket;
using Discord_Bot.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Jobs
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
                    .Select(bDay =>
                    {
                        var birthday = bDay.Item2;

                        var difference = new DateTime(today.Year, birthday.Month, birthday.Day) - today;

                        return (bDay , difference);
                    })
                    .Where(x => x.difference <= TimeSpan.FromDays(1) && x.difference > TimeSpan.Zero);

                if (birthdaysAndDifferences.Any())
                {
                    var guild = _client.GetGuild(serverId);
                    var events = await guild.GetEventsAsync();

                    foreach (var birthdayAndDifference in birthdaysAndDifferences)
                    {
                        var birthday = birthdayAndDifference.bDay;
                        var difference = birthdayAndDifference.difference;
                        var user = await _client.GetUserAsync(birthday.Item1);

                        if (!events.Any(x => x.Description.Contains(user.Username)))
                        {
                            try
                            {
                                await guild.CreateEventAsync(name: $"{user.Username}'s birthday",
                                                             startTime: new DateTimeOffset(today.AddTicks(difference.Ticks)),
                                                             type: GuildScheduledEventType.External,
                                                             description: $"It's {user.Username}'s birthday",
                                                             endTime: new DateTimeOffset(today.AddDays(1).AddTicks(difference.Ticks)),
                                                             location: "Here");
                            }
                            catch (Exception ex)
                            {
                                if (serverConfig.ModChannelID.HasValue)
                                {
                                    var channel = await _client.GetChannelAsync(serverConfig.ModChannelID.Value);
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
