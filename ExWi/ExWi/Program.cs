using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ExWi.Events;
using ExWi.Services;
using Microsoft.Extensions.DependencyInjection;

RunAsync().GetAwaiter().GetResult();

async Task RunAsync()
{
    var services = new ServiceCollection();
    RegisterServices(services);

    var provider = services.BuildServiceProvider();
    provider.GetRequiredService<LoggingService>();
    provider.GetRequiredService<ConfigService>();

    await provider.GetRequiredService<StartupService>().StartAsync();
    await Task.Delay(-1);
}

void RegisterServices(IServiceCollection services)
{
    //Client
    services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
    {
        LogLevel = LogSeverity.Info,
        MessageCacheSize = 100
    }))
    .AddSingleton(s => new InteractionService(s.GetRequiredService<DiscordSocketClient>()))
    //Events
    .AddSingleton<GuildEvent>()
    .AddSingleton<MessageEvent>()
    .AddSingleton<InteractionEvent>()
    .AddSingleton<ReadyEvent>()
    //Services
    .AddSingleton<ConfigService>()
    .AddSingleton<LoggingService>()
    .AddSingleton<StartupService>()
    .AddSingleton<EventService>()
    //HttpClientFactory
    .AddHttpClient();
}