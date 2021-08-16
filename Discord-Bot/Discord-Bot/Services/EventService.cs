using Discord.WebSocket;
using Discord_Bot.Events;

namespace Discord_Bot.Services
{
    public class EventService
    {
        private readonly DiscordSocketClient _client;
        private readonly MessageEvents _messageEvents;
        private readonly GuildEvents _guildEvents;
        private readonly InteractionEvents _interactionEvents;

        public EventService(
            DiscordSocketClient client,
            MessageEvents messageEvents,
            GuildEvents guildEvents,
            InteractionEvents interactionEvents)
        {
            _client = client;
            _messageEvents = messageEvents;
            _guildEvents = guildEvents;
            _interactionEvents = interactionEvents;
        }

        public void RegisterEvents()
        {
            _client.JoinedGuild += _guildEvents.JoinedGuild;

            _client.MessageUpdated += _messageEvents.MessageUpdated;
            _client.MessageDeleted += _messageEvents.MessageDeleted;
            _client.MessageReceived += _messageEvents.MessageReceived;

            _client.InteractionCreated += _interactionEvents.InteractionCreated;
        }
    }
}
