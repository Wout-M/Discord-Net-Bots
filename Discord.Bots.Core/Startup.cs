﻿using Discord.Bots.Core.Events;
using Discord.Bots.Core.Jobs;
using Discord.Bots.Core.Services;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Bots.Core;

public static class Startup
{
    public async static Task RunAsync(Type[] modules)
    {
        var services = new ServiceCollection();
        RegisterServices(services);

        var provider = services.BuildServiceProvider();
        provider.GetRequiredService<LoggingService>();
        provider.GetRequiredService<ConfigService>();

        await provider.GetRequiredService<StartupService>().StartAsync(modules);
        await Task.Delay(-1);
    }

    private static void RegisterServices(IServiceCollection services)
    {
        //Client
        services
              .AddSingleton(s => new InteractionService(s.GetRequiredService<DiscordSocketClient>(), new InteractionServiceConfig()
              {
                  LogLevel = LogSeverity.Info,
                  DefaultRunMode = RunMode.Async,
              }))
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 100,
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildBans | GatewayIntents.GuildEmojis |
                                 GatewayIntents.GuildIntegrations | GatewayIntents.GuildWebhooks | GatewayIntents.GuildVoiceStates |
                                 GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildMessageTyping | 
                                 GatewayIntents.DirectMessages | GatewayIntents.DirectMessageReactions | GatewayIntents.DirectMessageTyping | 
                                 GatewayIntents.AutoModerationConfiguration | GatewayIntents.AutoModerationActionExecution | GatewayIntents.GuildMessagePolls | 
                                 GatewayIntents.DirectMessagePolls | GatewayIntents.GuildMembers | GatewayIntents.MessageContent, 
            }))
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

        if (Config.Config.UseBirthdayChecking)
        {
            //Quartz
            services
                .AddScoped<BirthdayJob>()
                .AddQuartz();
        }
    }
}
