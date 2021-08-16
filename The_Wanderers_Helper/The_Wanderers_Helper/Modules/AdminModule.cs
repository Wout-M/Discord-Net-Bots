using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using The_Wanderers_Helper.Services;

namespace The_Wanderers_Helper.Modules
{
    [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    [Name("Admin")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private readonly ConfigService _configService;

        public AdminModule(ConfigService configService)
        {
            _configService = configService;
        }

        #region Prefix

        [Command("prefix")]
        [Summary("Change the prefix of the bot in this server")]
        public async Task Prefix([Summary("new prefix")] string prefix)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            config.Prefix = prefix;
            await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);

            await Context.Channel.SendMessageAsync($"Prefix has been successfully updated to `{prefix}`");
        }

        #endregion

        #region ModChannel

        [Command("modchannel")]
        [Alias("mc","adminchannel")]
        [Summary("Configure the admin channel")]
        public async Task ModChannel([Summary("channel")] ITextChannel channel)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            config.ModChannel = channel.Id;
            await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);

            await Context.Channel.SendMessageAsync($"{channel.Mention} has been configured as mod channel");
        }

        #endregion

        #region Check

        [Command("check")]
        [Summary("Check the bots configurations")]
        public async Task Check()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithTitle("My configurations")
                .AddField("Prefix", $"`{config.Prefix}`", true)
                .WithFooter("Created by Wout");

            string modChannelText = "No channel has been configured";
            embed.WithColor(Color.Red);
            if (config.ModChannel.HasValue)
            {
                var channel = Context.Guild.GetTextChannel(config.ModChannel.Value);
                if (channel != null)
                {
                    modChannelText = $"{channel.Mention}";
                    embed.WithColor(Color.Green);
                }
            }
            embed.AddField("Mod channel", modChannelText, true);

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        #endregion

        #region Purge

        [Command("purge")]
        [Summary("Delete <number> amount of messages")]
        public async Task Purge([Summary("number")] int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);

            var msg = await Context.Channel.SendMessageAsync($"I deleted {amount} messages");
            await Task.Delay(4000);
            await msg.DeleteAsync();
        }

        #endregion
    }
}
