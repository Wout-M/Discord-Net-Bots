﻿using Discord.Bots.Core.Services;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Discord.Bots.Core.Events;

public class ReadyEvent(
    IServiceProvider provider,
    DiscordSocketClient client,
    InteractionService interactionService,
    ConfigService configService)
{
    private readonly IServiceProvider _provider = provider;
    private readonly IScheduler? _scheduler = Config.Config.UseBirthdayChecking ? provider.GetRequiredService<IScheduler>() : null;
    private readonly DiscordSocketClient _client = client;
    private readonly InteractionService _interactionService = interactionService;
    private readonly ConfigService _configService = configService;

    public async Task Ready()
    {
        await _client.SetGameAsync(Config.Config.ActivityMessage, type: Config.Config.ActivityType);
        await _client.SetStatusAsync(UserStatus.DoNotDisturb);

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
            await _interactionService.RegisterCommandsToGuildAsync(serverConfig.Key);

            Console.WriteLine(new string('=', server.Name.Length) + "\n");
        }

        if (Config.Config.UseBirthdayChecking && _scheduler != null)
        {
            Console.WriteLine("Starting birthday checking...");

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
