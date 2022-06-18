using System.Collections.Generic;

namespace The_Wanderers_Helper.Config
{
    public class BotConfig
    {
        public BotConfig()
        {
            Servers = new();
        }

        public string Token { get; set; }
        public ulong OwnerID { get; set; }
        public Dictionary<ulong, ServerConfig> Servers { get; set; }
    }
}
