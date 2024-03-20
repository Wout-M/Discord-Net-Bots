using Discord.Bots.Core.Services;
using Discord.Interactions;
using Discord.WebSocket;

namespace Discord.Bots.Core.Modules;

public class ButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly ConfigService _configService;

    public ButtonModule(ConfigService configService)
    {
        _configService = configService;
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
        if (Config.Config.AllowSortRoleRemoval && user.RoleIds.Any(r => r == roleId))
        {
            await user.RemoveRoleAsync(roleId);
            await RespondAsync($"{user.Mention}, you already had this role. This role has been removed", ephemeral: true);
            return;
        }

        if (!Config.Config.AllowMultipleSortRoles && user.RoleIds.Any(r => config.SortRoles.Contains(r)))
        {
            await RespondAsync($"{user.Mention}, you have already been sorted. If you wish to change your guild, please contact one of the wardens", ephemeral: true);
            return;
        }

        var role = Context.Guild.GetRole(roleId);
        await user.AddRoleAsync(roleId);
        await RespondAsync($"{user.Mention}, you have been successfully sorted into {role.Mention}", ephemeral: true);
    }
}
