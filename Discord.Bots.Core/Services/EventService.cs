using Discord.Bots.Core.Events;
using Discord.WebSocket;

namespace Discord.Bots.Core.Services;

public class EventService
{
    private readonly DiscordSocketClient _client;
    private readonly GuildEvent _guildEvent;
    private readonly InteractionEvent _interactionEvent;
    private readonly MessageEvent _messageEvent;
    private readonly ReadyEvent _readyEvent;

    public EventService(
        DiscordSocketClient client,
        GuildEvent guildEvent,
        InteractionEvent interactionEvent,
        MessageEvent messageEvent,
        ReadyEvent readyEvent)
    {
        _client = client;
        _guildEvent = guildEvent;
        _interactionEvent = interactionEvent;
        _messageEvent = messageEvent;
        _readyEvent = readyEvent;
    }

    public void RegisterEvents()
    {
        _client.Ready += _readyEvent.Ready;
        _client.JoinedGuild += _guildEvent.JoinedGuild;

        _client.MessageUpdated += _messageEvent.MessageUpdated;
        _client.MessageDeleted += _messageEvent.MessageDeleted;
        _client.MessageReceived += _messageEvent.MessageReceived;

        _client.SlashCommandExecuted += _interactionEvent.HandleInteraction;
        _client.ButtonExecuted += _interactionEvent.HandleButtonInteraction;
    }
}
