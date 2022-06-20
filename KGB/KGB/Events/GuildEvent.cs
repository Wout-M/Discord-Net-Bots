using Discord.WebSocket;
using KGB.Config;
using KGB.Services;

namespace KGB.Events
{
    public class GuildEvent
    {
        private readonly ConfigService _configService;

        public GuildEvent(ConfigService configService)
        {
            _configService = configService;
        }

        public async Task JoinedGuild(SocketGuild guild)
        {
            var config = await _configService.GetConfig();

            if (!config.Servers.Keys.Contains(guild.Id))
            {
                var serverConfig = new ServerConfig()
                {
                    Name = guild.Name,
                };
                await _configService.AddOrUpdateServerConfig(guild.Id, serverConfig);
            }

            Console.WriteLine($"Joined {guild.Name} at {DateTime.Now}");
        }
    }
}
