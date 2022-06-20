using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace KGB.Services
{
    public class StartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _configService;
        private readonly EventService _eventService;
        private readonly InteractionService _interactions;

        public StartupService(
            IServiceProvider provider,
            DiscordSocketClient client,
            ConfigService configService,
            EventService eventService,
            InteractionService interactions)
        {
            _provider = provider;
            _client = client;
            _configService = configService;
            _eventService = eventService;
            _interactions = interactions;
        }

        public async Task StartAsync()
        {
            string discordToken = (await _configService.GetConfig()).Token;

            _eventService.RegisterEvents();
            await _client.LoginAsync(TokenType.Bot, discordToken);
            await _client.StartAsync();

            await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
    }
}
