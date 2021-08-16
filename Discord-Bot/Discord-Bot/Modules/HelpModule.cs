using Discord;
using Discord.Commands;
using Discord_Bot.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_Bot.Modules
{
    [Name("Help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;
        private readonly ConfigService _configService;

        public HelpModule(CommandService commands, ConfigService configService)
        {
            _commands = commands;
            _configService = configService;
        }


        [Command("help")]
        [Alias("h", "commands")]
        [Summary("List all my commands or more info about a specific command")]
        public async Task HelpAsync(
            [Summary("An (optional) command")]
            string command = null)
        {
            EmbedBuilder embed;
            if (!string.IsNullOrEmpty(command))
            {
                var result = _commands.Search(Context, command);
                embed = await GetCommandEmbed(result, command);

                if (embed == null)
                {
                    await ReplyAsync($"Sorry, I couldn't find a command like `{command}`");
                    return;
                }

            }
            else embed = await GetListEmbed();

            await ReplyAsync(embed: embed.Build());
        }

        private async Task<EmbedBuilder> GetListEmbed()
        {
            var serverConfig = await _configService.GetServerConfig(Context.Guild.Id);
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "Here's a list of all my commands"
            };

            foreach (var module in _commands.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += $"`{serverConfig?.Prefix}{cmd.Name}` : {cmd.Summary}\n";
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }
            return builder;
        }

        private async Task<EmbedBuilder> GetCommandEmbed(SearchResult result, string command)
        {
            var serverConfig = await _configService.GetServerConfig(Context.Guild.Id);
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are some commands like `{command}`"
            };

            if (result.IsSuccess)
            {
                foreach (var match in result.Commands)
                {
                    var cmd = match.Command;

                    var condition = await cmd.CheckPreconditionsAsync(Context);
                    if (condition.IsSuccess)
                    {
                        List<string> texts = new List<string> { $"{cmd.Summary}\n" };
                        string usage = $"{serverConfig?.Prefix}{cmd.Name}";

                        if (cmd.Aliases.Any()) texts.Add($"**Aliases:** {string.Join(", ", cmd.Aliases)}");
                        if (cmd.Parameters.Any())
                        {
                            texts.Add($"**Parameters:**\n- {string.Join("\n- ", cmd.Parameters.Select(p => $"`{p.Name}` : {p.Summary}"))}");

                            var parameters = cmd.Parameters.Select(param =>
                            {
                                return param.IsOptional ? $"[{param.Name}]" : param.Name;
                            });
                            usage += $" {string.Join(" ", parameters)}";
                        }
                        texts.Add($"**Usage:** `{usage}`");

                        builder.AddField(x =>
                        {
                            x.Name = cmd.Name;
                            x.Value = string.Join("\n", texts);
                            x.IsInline = false;
                        });
                    }
                }
            }

            return builder.Fields.Any() ? builder : null;
        }
    }
}
