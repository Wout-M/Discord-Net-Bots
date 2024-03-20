using Discord.Bots.Core.Services;
using Discord.Interactions;

namespace Discord.Bots.Core.Modules
{
    [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    [Group("sort", "Sorting roles")]
    public class SortModule : InteractionModuleBase<InteractionContext>
    {
        private readonly ConfigService _configService;

        public SortModule(ConfigService configService)
        {
            _configService = configService;
        }

        [SlashCommand("add", "Add a new role")]
        public async Task Add([Summary("role", "A role that should be added to the sorting menu")] IRole role)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);

            if (!config.SortRoles.Contains(role.Id))
            {
                config.SortRoles.Add(role.Id);
                await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
                await RespondAsync($"Successfully added `{role.Name}` to the sorting menu");
            }
            else
            {
                await RespondAsync($"`{role.Name}` is already a sorting role");
            }
        }

        [SlashCommand("remove", "Remove a role")]
        public async Task Remove([Summary("role", "A role that should be removed from the sorting menu")] IRole role)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);

            if (!config.SortRoles.Contains(role.Id))
            {
                await RespondAsync($"`{role.Name}` is not a sorting role");
            }
            else
            {
                config.SortRoles.Remove(role.Id);
                await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
                await RespondAsync($"Successfully removed `{role.Name}` from the sorting menu");

            }
        }

        [SlashCommand("list", "List the sorting roles")]
        public async Task List()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            var roles = config.SortRoles.Select(x => Context.Guild.GetRole(x));

            string text = roles.Any()
                ? string.Join("\n", roles.Select(r => $"- {r.Mention}"))
                : "No roles";

            var embedBuilder = new EmbedBuilder()
               .WithDescription("Here are the roles")
               .WithColor(Color.DarkGreen)
               .AddField("Roles", text)
               .WithFooter("Created by Wout");

            await RespondAsync(embed: embedBuilder.Build());
        }

        [SlashCommand("menu", "Display the sorting menu")]
        public async Task Sort()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);

            var roles = config.SortRoles.Select(x => Context.Guild.GetRole(x)).ToList();

            if (!roles.Any())
            {
                await RespondAsync("No sorting roles configured");
                return;
            }


            var rolesEmbed = new EmbedBuilder()
                .WithTitle(Config.Config.SortMenuTitle)
                .WithDescription(Config.Config.SortMenuDescription)
                .WithColor(Color.DarkBlue)
                .WithFooter("Created by Wout");

            var buttons = new ComponentBuilder();
            for (int i = 0; i < roles.Count; i++)
            {
                var button = new ButtonBuilder()
                {
                    Label = roles[i].Name,
                    CustomId = $"sortrole-{roles[i].Id}",
                    Style = (ButtonStyle)(((i + 1) % 4) + 1)
                };
                buttons.WithButton(button, i % 2);
            }

            await RespondAsync(embed: rolesEmbed.Build(), components: buttons.Build());
        }
    }
}
