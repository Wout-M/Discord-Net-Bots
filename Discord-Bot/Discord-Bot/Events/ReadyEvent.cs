using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord_Bot.Services;
using Quartz;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord_Bot.Events
{
    public class ReadyEvent
    {
        private readonly IServiceProvider _provider;
        private readonly IScheduler _scheduler;
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _configService;

        public ReadyEvent(
            IServiceProvider provider,
            IScheduler scheduler,
            DiscordSocketClient client,
            ConfigService configService)
        {
            _provider = provider;
            _scheduler = scheduler;
            _client = client;
            _configService = configService;
        }


        public async Task Ready()
        {
            await _client.SetGameAsync("to see if everyone behaves", type: Discord.ActivityType.Watching);
            await _client.SetStatusAsync(Discord.UserStatus.DoNotDisturb);


            var interactions = new InteractionService(_client, new InteractionServiceConfig()
            {
                LogLevel = LogSeverity.Info,
                DefaultRunMode = Discord.Interactions.RunMode.Async
            });
            await interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
            

            var config = await _configService.GetConfig();

            foreach (var serverConfig in config.Servers)
            {
                var server = serverConfig.Value;

                Console.ResetColor();
                Console.WriteLine(new string('=', server.Name.Length));
                Console.WriteLine(server.Name);
                Console.WriteLine(new string('=', server.Name.Length));

                if (server.ModChannelID.HasValue)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Mod channel configured");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Add a mod channel");
                }

                Console.ResetColor();
                Console.WriteLine("Register interactions...");
                await interactions.RegisterCommandsToGuildAsync(serverConfig.Key);

                Console.WriteLine(new string('=', server.Name.Length) + "\n");
            }

            var jobKey = new JobKey("job1", "group1");
            var trigger = TriggerBuilder.Create()
                .ForJob(jobKey)
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            var triggers = await _scheduler.GetTriggersOfJob(jobKey);
            if (!triggers.Any())
            {
                await _scheduler.ScheduleJob(trigger);
                Console.WriteLine("Started checking for birthdays");
            }
            else
            {
                Console.WriteLine("Birthday checking is already running");
            }
        }
    }
}
