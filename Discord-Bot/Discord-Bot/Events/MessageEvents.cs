using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Config;
using Discord_Bot.Services;
using System;
using System.Threading.Tasks;

namespace Discord_Bot.Events
{
    public class MessageEvents
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly ConfigService _configService;
        private readonly IServiceProvider _provider;

        public MessageEvents(
            DiscordSocketClient client,
            CommandService commands,
            ConfigService configService,
            IServiceProvider provider)
        {
            _client = client;
            _commands = commands;
            _configService = configService;
            _provider = provider;
        }

        #region Logging

        public async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            if (!(channel is IGuildChannel guildChannel)) return;
            if (after.Author.IsBot || after.Author.IsWebhook) return;

            var message = await before.GetOrDownloadAsync();
            var embed = new EmbedBuilder()
                .AddField("Created", message.Timestamp.DateTime.ToString())
                .AddField("Edited", after.EditedTimestamp.Value.DateTime.ToString(), true)
                .AddField("Author", after.Author.ToString())
                .AddField("Channel", channel.Name, true)
                .AddField("Old Message", message.Content)
                .AddField("New Message", after.Content)
                .WithTitle("Message updated")
                .WithColor(new Color(100, 100, 100))
                .WithFooter(footer => footer.Text = "Created by the almighty ginger")
                .WithAuthor(_client.CurrentUser)
                .WithCurrentTimestamp();

            await SendLogEmbed(embed.Build(), channel, guildChannel);
        }

        public async Task MessageDeleted(Cacheable<IMessage, ulong> deleted, ISocketMessageChannel channel)
        {
            if (!(channel is IGuildChannel guildChannel)) return;
            if (deleted.HasValue)
            {
                if (deleted.Value.Author.IsBot || deleted.Value.Author.IsWebhook) return;
                var embed = new EmbedBuilder()
               .AddField("Deleted", DateTime.Now.ToString())
               .AddField("Author", deleted.Value.Author.ToString())
               .AddField("Channel", channel.Name, true)
               .AddField("Message", deleted.Value.Content)
               .WithTitle("Message deleted")
               .WithColor(new Color(100, 100, 100))
               .WithFooter(footer => footer.Text = "Created by the almighty ginger")
               .WithAuthor(_client.CurrentUser)
               .WithCurrentTimestamp();

                await SendLogEmbed(embed.Build(), channel, guildChannel);
            }
        }

        private async Task SendLogEmbed(Embed embed, ISocketMessageChannel channel, IGuildChannel guildChannel)
        {
            var config = await _configService.GetConfig();

            if (config.Servers.TryGetValue(guildChannel.GuildId, out ServerConfig serverConfig) && serverConfig.LogChannelID != 0)
            {
                var logChannel = await guildChannel.Guild.GetTextChannelAsync(serverConfig.LogChannelID);
                await logChannel?.SendMessageAsync(embed: embed);
            }
            else
            {
                string text = "Log channel not configured for this server!";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(text);
                Console.ForegroundColor = ConsoleColor.White;
                await channel.SendMessageAsync(text);
            }
        }

        #endregion

        #region CommandHandler

        public async Task MessageReceived(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;
            if (!(msg.Channel is ITextChannel)) return;

            var context = new SocketCommandContext(_client, msg);
            var config = await _configService.GetConfig();
            var prefix = config.Servers.TryGetValue(context.Guild.Id, out ServerConfig serverConfig) ? serverConfig.Prefix : config.Prefix;

            int argPos = 0;
            if (msg.HasCharPrefix(prefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);
                if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ToString());
            }
        }

        #endregion
    }
}
