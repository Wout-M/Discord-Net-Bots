using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ExWi.Services;

namespace ExWi.Modules
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
                await RespondAsync("Something went wrong with the role id, please contact one of the admins", ephemeral: true);
                return;
            }
            if (Context.User is not IGuildUser user)
            {
                await RespondAsync("Something went wrong with the user convertion, please contact one of the admins", ephemeral: true);
                return;
            }

            var config = await _configService.GetServerConfig(Context.Guild.Id);
            if (user.RoleIds.Any(r => r == roleId))
            {
                await user.RemoveRoleAsync(roleId);
                await RespondAsync($"{user.Mention}, you already had this role. This role has been removed", ephemeral: true);
                return;
            }

            var role = Context.Guild.GetRole(roleId);
            await user.AddRoleAsync(roleId);
            await RespondAsync($"{user.Mention}, you have been successfully sorted into {role.Mention}", ephemeral: true);
        }
    }
}
