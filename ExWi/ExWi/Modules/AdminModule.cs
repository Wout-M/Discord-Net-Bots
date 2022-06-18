using Discord;
using Discord.Interactions;
using ExWi.Services;

namespace ExWi.Modules
{
    [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public class AdminModule : InteractionModuleBase<InteractionContext>
    {
        private readonly ConfigService _configService;

        public AdminModule(ConfigService configService)
        {
            _configService = configService;
        }

        #region ModChannel

        [SlashCommand("mod", "Configure the mod channel")]
        public async Task ModChannel(ITextChannel channel)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            config.ModChannel = channel.Id;
            await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);

            await RespondAsync($"{channel.Mention} has been configured as mod channel");
        }

        #endregion

        #region Check

        [SlashCommand("check", "Check the bots configurations")]
        public async Task Check()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithTitle("My configurations")
                .WithFooter("Created by Wout");

            string modChannelText = "No channel has been configured";
            embed.WithColor(Color.Red);
            if (config.ModChannel.HasValue)
            {
                var channel = await Context.Guild.GetTextChannelAsync(config.ModChannel.Value);
                if (channel != null)
                {
                    modChannelText = $"{channel.Mention}";
                    embed.WithColor(Color.Green);
                }
            }
            embed.AddField("Mod channel", modChannelText);

            await RespondAsync(embed: embed.Build());
        }

        #endregion
    }
}
