using System.Collections.Generic;

namespace The_Wanderers_Helper.Config
{
    public class BotConfig
    {
        public BotConfig()
        {
            Servers = new Dictionary<ulong, ServerConfig>();
        }

        public string Token { get; set; }
        public ulong OwnerID { get; set; }
        public string Prefix { get; set; }
        public Dictionary<ulong, ServerConfig> Servers { get; set; }
    }
}
