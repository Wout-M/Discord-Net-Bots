using Discord.Bots.Core.Events;
using Discord.WebSocket;

namespace Discord.Bots.Core.Services;

public class EventService(
    DiscordSocketClient client,
    GuildEvent guildEvent,
    InteractionEvent interactionEvent,
    MessageEvent messageEvent,
    ReadyEvent readyEvent)
{
    private readonly DiscordSocketClient _client = client;
    private readonly GuildEvent _guildEvent = guildEvent;
    private readonly InteractionEvent _interactionEvent = interactionEvent;
    private readonly MessageEvent _messageEvent = messageEvent;
    private readonly ReadyEvent _readyEvent = readyEvent;

    public void RegisterEvents()
    {
        _client.Ready += _readyEvent.Ready;
        _client.JoinedGuild += _guildEvent.JoinedGuild;

        if (Config.Config.UseMessageLogging)
        {
            _client.MessageUpdated += _messageEvent.MessageUpdated;
            _client.MessageDeleted += _messageEvent.MessageDeleted;
        }

        if (Config.Config.UseWordsChecking)
        {
            _client.MessageReceived += _messageEvent.MessageReceived;
        }

        _client.SlashCommandExecuted += _interactionEvent.HandleInteraction;
        _client.ButtonExecuted += _interactionEvent.HandleButtonInteraction;
    }
}
