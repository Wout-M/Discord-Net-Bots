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
    [Group("sort", "Sorting")]
    public class SortModule : InteractionModuleBase<InteractionContext>
    {
        private readonly ConfigService _configService;

        public SortModule(ConfigService configService)
        {
            _configService = configService;
        }


        [SlashCommand("menu", "Display the sorting menu")]
        public async Task Menu()
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("Choose the roles you want")
                .WithDescription("Click on the button with your role")
               .WithColor(new Color(78, 91, 245))
               .WithFooter("Created by Wout");


            var roles = new ComponentBuilder();

            for (int i = 0; i < 4; i++)
            {
                var button = new ButtonBuilder()
                {
                    Label = $"test-{i}",
                    CustomId = $"sortrole-{i}",
                    Style = (ButtonStyle)(((i + 1) % 4) + 1)
                };
                roles.WithButton(button, i % 2);
            }

            await RespondAsync(embed: embedBuilder.Build(), components: roles.Build());
        }

        [SlashCommand("add", "Add a new role")]
        public async Task Add(IRole role)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            if (!config.SortRoles.Contains(role.Id))
            {
                config.SortRoles.Add(role.Id);
                await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
                await RespondAsync($"You added **{role.Name}** to the sorting roles");
            }
            else
            {
                await RespondAsync($"**{role.Name}** is already a sorting role");
            }   
        }

        [SlashCommand("remove", "Remove a role")]
        public async Task Remove(IRole role)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            if (!config.SortRoles.Contains(role.Id))
            {
                await RespondAsync($"**{role.Name}** is already a sorting role");
            }
            else
            {
                config.SortRoles.Add(role.Id);
                await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
                await RespondAsync($"You added **{role.Name}** to the sorting roles");
                
            }
        }
    }
}
