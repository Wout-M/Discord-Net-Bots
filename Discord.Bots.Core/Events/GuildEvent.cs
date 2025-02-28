﻿using Discord.Bots.Core.Models;
using Discord.Bots.Core.Services;
using Discord.Interactions;
using Discord.WebSocket;

namespace Discord.Bots.Core.Events;

public class GuildEvent(ConfigService configService, InteractionService interactionService)
{
    private readonly ConfigService _configService = configService;
    private readonly InteractionService _interactionService = interactionService;

    public async Task JoinedGuild(SocketGuild guild)
    {
        var config = await _configService.GetConfig();

        if (!config.Servers.ContainsKey(guild.Id))
        {
            var serverConfig = new Server()
            {
                Name = guild.Name,
            };

            await _configService.AddOrUpdateServerConfig(guild.Id, serverConfig);
            await _interactionService.RegisterCommandsToGuildAsync(guild.Id);
        }

        Console.WriteLine($"Joined {guild.Name} at {DateTime.Now}");
    }
}
