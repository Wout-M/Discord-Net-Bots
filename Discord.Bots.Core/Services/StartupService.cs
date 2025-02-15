using Discord.Interactions;
using Discord.WebSocket;

namespace Discord.Bots.Core.Services;

public class StartupService(
    IServiceProvider provider,
    DiscordSocketClient client,
    ConfigService configService,
    EventService eventService,
    InteractionService interactions)
{
    private readonly IServiceProvider _provider = provider;
    private readonly DiscordSocketClient _client = client;
    private readonly ConfigService _configService = configService;
    private readonly EventService _eventService = eventService;
    private readonly InteractionService _interactions = interactions;

    public async Task StartAsync(Type[] modules)
    {
        string discordToken = (await _configService.GetConfig()).Token;

        _eventService.RegisterEvents();
        await _client.LoginAsync(TokenType.Bot, discordToken);
        await _client.StartAsync();

        foreach (var module in modules)
        {
            await _interactions.AddModuleAsync(module, _provider);
        }
    }
}
