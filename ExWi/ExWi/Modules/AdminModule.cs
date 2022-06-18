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

        #region LogChannel

        [SlashCommand("log", "Configure the log channel")]
        public async Task LogChannel(ITextChannel channel)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            config.LogChannel = channel.Id;
            await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);

            await RespondAsync($"{channel.Mention} has been configured as log channel");
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

            string noChannelText = "No channel has been configured";
            string modChannelText = noChannelText;
            if (config.ModChannel.HasValue)
            {
                var channel = await Context.Guild.GetTextChannelAsync(config.ModChannel.Value);
                if (channel != null)
                {
                    modChannelText = $"{channel.Mention}";
                }
            }
            embed.AddField("Mod channel", modChannelText);

            string logChannelText = noChannelText;
            if (config.ModChannel.HasValue)
            {
                var channel = await Context.Guild.GetTextChannelAsync(config.ModChannel.Value);
                if (channel != null)
                {
                    logChannelText = $"{channel.Mention}";
                }
            }
            embed.AddField("Log channel", logChannelText);

            embed.WithColor((modChannelText != noChannelText && logChannelText != noChannelText) ? Color.Green : Color.Red);

            await RespondAsync(embed: embed.Build());
        }

        #endregion
    }
}
