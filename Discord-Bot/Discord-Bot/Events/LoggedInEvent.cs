using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord_Bot.Services;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord_Bot.Events
{
    public class LoggedInEvent
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _configService;

        public LoggedInEvent(IServiceProvider provider, DiscordSocketClient client, ConfigService configService)
        {
            _provider = provider;
            _client = client;
            _configService = configService;
        }

        public async Task LoggedIn()
        {
            //var config = await _configService.GetConfig();

            //var interactions = new InteractionService(_client, new InteractionServiceConfig()
            //{
            //    LogLevel = LogSeverity.Info,
            //    DefaultRunMode = Discord.Interactions.RunMode.Async
            //});
            //await interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

            //Console.WriteLine("Register interactions...");
            //foreach (var serverConfig in config.Servers)
            //{
            //    Console.WriteLine(serverConfig.Value.Name);
            //    await interactions.RegisterCommandsToGuildAsync(serverConfig.Key);
            //}
            
        }
    }
}
