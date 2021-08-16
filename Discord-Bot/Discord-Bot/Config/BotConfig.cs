using System.Collections.Generic;

namespace Discord_Bot.Config
{
    public class BotConfig
    {
        public BotConfig()
        {
            Servers = new Dictionary<ulong, ServerConfig>();
        }

        public string Token { get; set; }
        public ulong OwnerID { get; set; }
        public char Prefix { get; set; }
        public Dictionary<ulong, ServerConfig> Servers { get; set; }
    }
}
