using Discord.WebSocket;
using The_Wanderers_Helper.Events;

namespace The_Wanderers_Helper.Services
{
    public class EventService
    {
        private readonly DiscordSocketClient _client;
        private readonly GuildEvent _guildEvent;
        private readonly InteractionEvent _interactionEvent;
        private readonly ReadyEvent _readyEvent;

        public EventService(
            DiscordSocketClient client,
            GuildEvent guildEvent,
            InteractionEvent interactionEvent,
            ReadyEvent readyEvent)
        {
            _client = client;
            _guildEvent = guildEvent;
            _interactionEvent = interactionEvent;
            _readyEvent = readyEvent;
        }

        public void RegisterEvents()
        {
            _client.Ready += _readyEvent.Ready;
            _client.JoinedGuild += _guildEvent.JoinedGuild;

            _client.SlashCommandExecuted += _interactionEvent.HandleInteraction;
            _client.ButtonExecuted += _interactionEvent.HandleButtonInteraction;
        }
    }
}
