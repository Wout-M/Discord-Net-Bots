using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using The_Wanderers_Helper.Services;

namespace The_Wanderers_Helper.Modules
{
    [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    [Name("Sorting")]
    [Group("sort")]
    [Summary("Sorting roles")]
    public class SortRoleModule : ModuleBase<SocketCommandContext>
    {
        private readonly ConfigService _configService;

        public SortRoleModule(ConfigService configService)
        {
            _configService = configService;
        }

        [Command("add")]
        [Summary("Add a new guild")]
        public async Task Add([Summary("role")] IRole role)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);

            if (!config.SortRoles.Contains(role.Id))
                config.SortRoles.Add(role.Id);

            await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
            await Context.Channel.SendMessageAsync($"Successfully added `{role.Name}` to the sorting guilds");
        }

        [Command("remove")]
        [Summary("Remove a guild")]
        public async Task Remove([Summary("role")] IRole role)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);

            if (config.SortRoles.Contains(role.Id))
                config.SortRoles.Remove(role.Id);

            await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
            await Context.Channel.SendMessageAsync($"Successfully removed `{role.Name}` from the sorting guilds");
        }

        [Command("list")]
        [Summary("List the sorting guilds")]
        public async Task List()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);

            var roles = config.SortRoles.Select(x => Context.Guild.GetRole(x));
            string text = roles.Any()
                ? string.Join("\n", roles.Select(r => $"- {r.Mention}"))
                : "No guilds";

            var rolesEmbed = new EmbedBuilder()
               .WithDescription("Here are the guilds")
               .WithColor(new Color(78, 91, 245))
               .AddField("Guilds", text);

            await Context.Channel.SendMessageAsync(embed: rolesEmbed.Build());
        }

        [Command]
        [Summary("Display the sorting buttons")]
        public async Task Sort()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);

            var roles = config.SortRoles.Select(x => Context.Guild.GetRole(x)).ToList();

            if (!roles.Any())
                await Context.Channel.SendMessageAsync("No sorting guilds configured");

            var rolesEmbed = new EmbedBuilder()
                .WithTitle("Choose a guild to get sorted in")
               .WithDescription("Click on the button with your guild")
               .WithColor(new Color(78, 91, 245))
               .WithFooter("Created by Wout");

            var buttons = new ComponentBuilder();
            for (int i = 0; i < roles.Count; i++)
            {
                var button = new ButtonBuilder()
                {
                    Label = roles[i].Name,
                    CustomId = $"sort_{roles[i].Id}",
                    Style = (ButtonStyle)(((i + 1) % 4) + 1)
                };
                buttons.WithButton(button, i % 2);
            }

            await Context.Channel.SendMessageAsync(embed: rolesEmbed.Build(), component: buttons.Build());
        }
    }
}
