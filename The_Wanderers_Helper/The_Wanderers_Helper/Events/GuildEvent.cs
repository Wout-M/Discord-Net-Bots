using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using The_Wanderers_Helper.Config;
using The_Wanderers_Helper.Services;

namespace The_Wanderers_Helper.Events
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
                    Prefix = config.Prefix
                };
                await _configService.AddOrUpdateServerConfig(guild.Id, serverConfig);
            }
        }
    }
}