using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using The_Wanderers_Helper.Config;

namespace The_Wanderers_Helper.Services
{
    public class CommandsService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly ConfigService _configService;
        private readonly IServiceProvider _provider;

        public CommandsService(
            DiscordSocketClient client,
            CommandService commands,
            ConfigService configService,
            IServiceProvider provider)
        {
            _client = client;
            _commands = commands;
            _configService = configService;
            _provider = provider;

            _commands.CommandExecuted += OnCommandExecuted;
        }

        public async Task HandleCommand(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;
            if (!(msg.Channel is ITextChannel)) return;

            int argPos = 0;
            if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, msg);
            var config = await _configService.GetConfig();
            var prefix = config.Servers.TryGetValue(context.Guild.Id, out ServerConfig serverConfig) ? serverConfig.Prefix : config.Prefix;

            if (msg.HasStringPrefix(prefix, ref argPos))
                await _commands.ExecuteAsync(context, argPos, _provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (result.Error.HasValue)
            {
                switch (result.Error.Value)
                {
                    //case CommandError.UnknownCommand:
                    //    break;
                    //case CommandError.ParseFailed:
                    //    break;
                    //case CommandError.BadArgCount:

                    //    break;
                    //case CommandError.ObjectNotFound:
                    //    break;
                    //case CommandError.MultipleMatches:
                    //    break;
                    case CommandError.UnmetPrecondition:
                        await context.Message.ReplyAsync($"{context.User.Mention}, you don't have permissions to use this command");
                        break;
                    case CommandError.Exception:
                        await context.Message.ReplyAsync($"{context.User.Mention}, an error occured while using this command");
                        break;
                    //case CommandError.Unsuccessful:
                    //    break;
                    //default:
                    //    break;
                }
            }
        }
    }
}
