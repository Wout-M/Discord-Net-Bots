using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using The_Wanderers_Helper.Services;

namespace The_Wanderers_Helper.Events
{
    public class ReadyEvent
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _configService;

        public ReadyEvent(DiscordSocketClient client, ConfigService configService)
        {
            _client = client;
            _configService = configService;
        }


        public async Task Ready()
        {
            await _client.SetGameAsync("to see if everyone behaves", type: Discord.ActivityType.Watching);
            await _client.SetStatusAsync(Discord.UserStatus.DoNotDisturb);

            var config = await _configService.GetConfig();

            foreach (var server in config.Servers.Values)
            {
                Console.ResetColor();
                Console.WriteLine(new string('=', server.Name.Length));
                Console.WriteLine(server.Name);
                Console.WriteLine(new string('=', server.Name.Length));

                if (server.ModChannel.HasValue)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Mod channel configured");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Add a mod channel");
                }

                Console.ResetColor();
                Console.WriteLine(new string('=', server.Name.Length) + "\n");
            }
        }
    }
}
