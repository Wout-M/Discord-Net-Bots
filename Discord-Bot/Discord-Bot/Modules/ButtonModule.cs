using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord_Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules
{
    public class ButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        private readonly ConfigService _configService;

        public ButtonModule(ConfigService configService)
        {
            _configService = configService;
        }

        [ComponentInteraction("sortrole-*")]
        public async Task Sort(string id)
        {
            if (!ulong.TryParse(id, out ulong roleId))
            {
                await RespondAsync("Something went wrong with the role id, please contact one of the wardens", ephemeral: true);
                return; 
            }
            if (Context.User is not IGuildUser user)
            {
                await RespondAsync("Something went wrong with the user convertion, please contact one of the wardens", ephemeral: true);
                return;
            }

            var config = await _configService.GetServerConfig(Context.Guild.Id);
            if (user.RoleIds.Any(r => config.SortRoles.Contains(r)))
            {
                await RespondAsync($"{user.Mention}, you have already been sorted. If you wish to change your guild, please contact one of the wardens", ephemeral: true);
                return;
            }

            var role = Context.Guild.GetRole(roleId);
            await user.AddRoleAsync(role);
            await RespondAsync($"{user.Mention}, you have been successfully sorted into {role.Mention}", ephemeral: true);
        }

        [ComponentInteraction("quiz_*_*")]
        public async Task Quiz(string correct, string answer)
        {
            if (!int.TryParse(correct, out int correctInt))
            {
                
                await RespondAsync("Something went wrong with the answer button, please contact one of the wardens", ephemeral: true);
                return;
            }

            bool correctBool = Convert.ToBoolean(correctInt);
            answer = answer.Replace("|", " ");
            string text = correctBool
                ? $"`{answer}` is correct! Congratulations {Context.Interaction.User.Mention}"
                : $"`{answer}` was not the correct answer, {Context.Interaction.User.Mention}";

            await Context.Interaction.UpdateAsync(x =>
            {
                x.Content = text;
                x.Components = null;
                x.Embeds = null;
            });
        }
    }
}
