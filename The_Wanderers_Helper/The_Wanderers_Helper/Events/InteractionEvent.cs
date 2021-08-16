using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Wanderers_Helper.Services;

namespace The_Wanderers_Helper.Events
{
    public class InteractionEvent
    {
        private readonly ConfigService _configService;

        public InteractionEvent(ConfigService configService)
        {
            _configService = configService;
        }

        public async Task InteractionCreated(SocketInteraction interaction)
        {
            switch (interaction)
            {
                case SocketSlashCommand commandInteraction:

                    break;

                case SocketMessageComponent componentInteraction:
                    await MyMessageComponentHandler(componentInteraction);
                    break;

                default:
                    break;
            }
        }

        private async Task MyMessageComponentHandler(SocketMessageComponent interaction)
        {
            switch (interaction.Data.CustomId)
            {
                case string s when s.StartsWith("quiz"):
                    await ProcessQuiz(interaction);
                    break;
                case string s when s.StartsWith("sort"):
                    await ProcessSort(interaction);
                    break;
                default:
                    break;
            }

        }

        public async Task ProcessQuiz(SocketMessageComponent clickedButton)
        {
            bool correct = clickedButton.Data.CustomId.EndsWith("correct");
            var actionRow = clickedButton.Message.Components.First(comp => comp.Components.Any(button => ((ButtonComponent)button).CustomId == clickedButton.Data.CustomId));
            string answer = ((ButtonComponent)actionRow.Components.FirstOrDefault(button => ((ButtonComponent)button).CustomId == clickedButton.Data.CustomId)).Label;
            string text = correct
                ? $"`{answer}` is correct! Congratulations {clickedButton.User.Mention}"
                : $"`{answer}` was not the correct answer, {clickedButton.User.Mention}";

            await clickedButton.Message.DeleteAsync();
            await clickedButton.Channel.SendMessageAsync(text);
        }

        public async Task ProcessSort(SocketMessageComponent button)
        {
            if (!(button.Channel is ITextChannel channel)) return;
            if (!(button.User is IGuildUser user)) return;

            var config = await _configService.GetServerConfig(channel.GuildId);
            if (user.RoleIds.Any(r => config.SortRoles.Contains(r)))
            {
                await button.FollowupAsync($"{user.Mention}, you have already been sorted. If you wish to change your guild, please contact one of the wardens", ephemeral: true);
                return;
            }

            ulong roleId = ulong.Parse(button.Data.CustomId.Substring(button.Data.CustomId.IndexOf("_") + 1));
            var role = channel.Guild.GetRole(roleId);

            await user.AddRoleAsync(roleId);
            await button.FollowupAsync($"{user.Mention}, you have been successfully sorted into {role.Mention}", ephemeral: true);
        }
    }
}
