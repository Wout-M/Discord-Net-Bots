using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord_Bot.Services
{
    public class StartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _configService;
        private readonly EventService _eventService;
        private readonly CommandService _commands;

        public StartupService(
            IServiceProvider provider,
            DiscordSocketClient client,
            ConfigService configService,
            EventService eventService,
            CommandService commands)
        {
            _provider = provider;
            _client = client;
            _configService = configService;
            _eventService = eventService;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            string discordToken = (await _configService.GetConfig()).Token;

            _eventService.RegisterEvents();
            await _client.LoginAsync(TokenType.Bot, discordToken);     
            await _client.StartAsync();                               

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);   
        }
    }
}
