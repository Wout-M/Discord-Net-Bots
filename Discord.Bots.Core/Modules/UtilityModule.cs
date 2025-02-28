﻿using Discord.Interactions;
using Discord.WebSocket;

namespace Discord.Bots.Core.Modules;

public class UtilityModule : InteractionModuleBase<InteractionContext>
{
    #region Ping

    [SlashCommand("ping", "pong")]
    public async Task Ping()
    {
        var latency = (Context.Client as DiscordSocketClient)?.Latency;
        await RespondAsync($"Pong `{latency} ms`");
    }

    #endregion
}
