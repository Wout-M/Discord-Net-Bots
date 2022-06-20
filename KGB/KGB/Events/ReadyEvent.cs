using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using KGB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KGB.Events
{
    public class ReadyEvent
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _configService;

        public ReadyEvent(IServiceProvider provider, DiscordSocketClient client, ConfigService configService)
        {
            _provider = provider;
            _client = client;
            _configService = configService;
        }


        public async Task Ready()
        {
            await _client.SetGameAsync("to see if everyone behaves", type: ActivityType.Watching);
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);

            var interactions = new InteractionService(_client, new InteractionServiceConfig()
            {
                LogLevel = LogSeverity.Info,
                DefaultRunMode = RunMode.Async
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

                if (server.ModChannel.HasValue)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Mod channel configured");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Add a mod channel");
                }

                if (server.LogChannel.HasValue)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Log channel configured");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Add a log channel");
                }

                Console.ResetColor();
                Console.WriteLine("Register interactions...");
                await interactions.RegisterCommandsToGuildAsync(serverConfig.Key);

                Console.WriteLine(new string('=', server.Name.Length) + "\n");
            }
        }
    }
}
