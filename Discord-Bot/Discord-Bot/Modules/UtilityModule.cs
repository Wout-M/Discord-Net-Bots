using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules;
public class UtilityModule : InteractionModuleBase<InteractionContext>
{
    #region Ping

    [SlashCommand("ping", "pong")]
    public async Task Ping()
    {
        var latency = (Context.Client as DiscordSocketClient).Latency;
        await RespondAsync($"Pong `{latency} ms`");
    }

    #endregion

    [SlashCommand("event", "Test event")]
    public async Task Event()
    {
        await Context.Guild.CreateEventAsync("test event",
                                             DateTime.Now.AddSeconds(2),
                                             Discord.GuildScheduledEventType.External,
                                             description: "test",
                                             endTime: DateTime.Now.AddMinutes(4),
                                             location: "test");
        await RespondAsync("test event created");
    }

}
