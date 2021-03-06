using Discord;
using Discord.WebSocket;
using ExWi.Config;
using ExWi.Services;

namespace ExWi.Events
{
    public class MessageEvent
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _configService;

        public MessageEvent(DiscordSocketClient client, ConfigService configService)
        {
            _client = client;
            _configService = configService;
        }

        public async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            if (!(channel is IGuildChannel guildChannel)) return;
            if (after.Author.IsBot || after.Author.IsWebhook) return;

            var message = await before.GetOrDownloadAsync();
            var embed = new EmbedBuilder()
                .AddField("Created", message.Timestamp.DateTime.ToString("G"))
                .AddField("Edited", after.EditedTimestamp!.Value.DateTime.ToString("G"), true)
                .AddField("Author", after.Author.ToString())
                .AddField("Channel", channel.Name, true)
                .AddField("Old Message", message.Content.Length > 1024 ? "This message is too long" : message.Content)
                .AddField("New Message", after.Content.Length > 1024 ? "This message is too long" : after.Content)
                .WithTitle("Message updated")
                .WithColor(Color.DarkBlue)
                .WithFooter(footer => footer.Text = "Created by the almighty ginger")
                .WithAuthor(_client.CurrentUser)
                .WithCurrentTimestamp();

            await SendLogEmbed(embed.Build(), channel, guildChannel);
        }

        public async Task MessageDeleted(Cacheable<IMessage, ulong> deleted, Cacheable<IMessageChannel, ulong> cachedChannel)
        {
            var channel = await cachedChannel.GetOrDownloadAsync();
            if (channel is not IGuildChannel guildChannel) return;

            if (deleted.HasValue)
            {
                if (deleted.Value.Author.IsBot || deleted.Value.Author.IsWebhook) return;
                var embed = new EmbedBuilder()
               .AddField("Deleted", deleted.Value.Timestamp.DateTime.ToString("G"))
               .AddField("Author", deleted.Value.Author.ToString())
               .AddField("Channel", channel.Name, true)
               .AddField("Message", deleted.Value.Content.Length > 1024 ? "This message is too long" : deleted.Value.Content)
               .WithTitle("Message deleted")
               .WithColor(Color.DarkRed)
               .WithFooter(footer => footer.Text = "Created by the almighty ginger")
               .WithAuthor(_client.CurrentUser)
               .WithCurrentTimestamp();

                await SendLogEmbed(embed.Build(), channel as ISocketMessageChannel, guildChannel);
            }
        }

        private async Task SendLogEmbed(Embed embed, ISocketMessageChannel channel, IGuildChannel guildChannel)
        {
            var config = await _configService.GetConfig();

            if (config.Servers.TryGetValue(guildChannel.GuildId, out ServerConfig serverConfig) && serverConfig.LogChannel.HasValue)
            {
                var logChannel = await guildChannel.Guild.GetTextChannelAsync(serverConfig.LogChannel.Value);
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
    }
}
