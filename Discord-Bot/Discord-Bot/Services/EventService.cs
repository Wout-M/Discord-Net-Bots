using Discord.WebSocket;
using Discord_Bot.Events;

namespace Discord_Bot.Services
{
    public class EventService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionEvent _interactionEvent;
        private readonly LoggedInEvent _loggedInEvent;
        private readonly MessageEvent _messageEvent;
        private readonly GuildEvent _guildEvent;
        private readonly ReadyEvent _readyEvent;

        public EventService(
            DiscordSocketClient client,
            InteractionEvent interactionEvent,
            LoggedInEvent loggedInEvent,
            MessageEvent messageEvent,
            GuildEvent guildEvent,
            ReadyEvent readyEvent)
        {
            _client = client;
            _interactionEvent = interactionEvent;
            _loggedInEvent = loggedInEvent;
            _messageEvent = messageEvent;
            _guildEvent = guildEvent;
            _readyEvent = readyEvent;
        }

        public void RegisterEvents()
        {
            _client.Ready += _readyEvent.Ready;
            _client.LoggedIn += _loggedInEvent.LoggedIn;
            _client.JoinedGuild += _guildEvent.JoinedGuild;

            _client.MessageUpdated += _messageEvent.MessageUpdated;
            _client.MessageDeleted += _messageEvent.MessageDeleted;

            _client.SlashCommandExecuted += _interactionEvent.HandleInteraction;
            _client.ButtonExecuted += _interactionEvent.HandleButtonInteraction;
        }
    }
}