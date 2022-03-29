using Discord;
using Discord.Interactions;
using Discord_Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules
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
                await RespondAsync($"Successfully added `{role.Name}` to the sorting roles");
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
                await RespondAsync($"Successfully removed `{role.Name}` from the sorting roles");

            }
        }

        [SlashCommand("list", "List the sorting roles")]
        public async Task List()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            var roles = config.SortRoles.Select(x => Context.Guild.GetRole(x));
            
            string text = roles.Any()
                ? string.Join("\n", roles.Select(r => $"- {r.Mention}"))
                : "No guilds";

            var embedBuilder = new EmbedBuilder()
               .WithDescription("Here are the guilds")
               .WithColor(new Color(78, 91, 245))
               .AddField("Guilds", text);

            await RespondAsync(embed: embedBuilder.Build());
        }

        [SlashCommand("menu", "Display the sorting menu")]
        public async Task Menu()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            var roles = config.SortRoles.Select(id => Context.Guild.GetRole(id)).ToList();

            if (!roles.Any())
            {
                await RespondAsync("No sorting roles configured");
            }
            else
            {
                var embedBuilder = new EmbedBuilder()
                    .WithTitle("Choose the roles you want")
                    .WithDescription("Click on the button with your role")
                    .WithColor(new Color(78, 91, 245))
                    .WithFooter("Created by Wout");

                var buttonBuilder = new ComponentBuilder();

                for (int i = 0; i < roles.Count; i++)
                {
                    var button = new ButtonBuilder()
                    {
                        Label = roles[i].Name,
                        CustomId = $"sortrole-{roles[i].Id}",
                        Style = (ButtonStyle)(((i + 1) % 4) + 1)
                    };
                    buttonBuilder.WithButton(button, i % 2);
                }

                await RespondAsync(embed: embedBuilder.Build(), components: buttonBuilder.Build());
            }
        }
    }
}
