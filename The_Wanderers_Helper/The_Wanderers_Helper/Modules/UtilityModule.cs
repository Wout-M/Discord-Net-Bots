using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace The_Wanderers_Helper.Modules
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
