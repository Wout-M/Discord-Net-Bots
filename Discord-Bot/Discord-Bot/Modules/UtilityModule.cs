using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules
{
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
    }
}
