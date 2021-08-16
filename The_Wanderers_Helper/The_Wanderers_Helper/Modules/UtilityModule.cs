using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace The_Wanderers_Helper.Modules
{
    [Name("Utility")]
    public class UtilityModule : ModuleBase<SocketCommandContext>
    {
        #region Ping

        [Command("ping")]
        [Summary("pong")]
        public async Task Ping()
        {
            var msg = await Context.Message.ReplyAsync("Pong");
            await msg.ModifyAsync(x => x.Content = $"Pong `{(msg.Timestamp - Context.Message.Timestamp).Milliseconds} ms`");
        }

        #endregion
    }
}
