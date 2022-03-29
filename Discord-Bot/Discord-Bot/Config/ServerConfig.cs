using System.Collections.Generic;

namespace Discord_Bot.Config
{
    public class ServerConfig
    {
        public ServerConfig()
        {
            SortRoles = new List<ulong>();
        }

        public string Name { get; set; }
        public string Prefix { get; set; }
        public ulong? ModChannelID { get; set; }
        public ulong? LogChannelID { get; set; }
        public List<ulong> SortRoles { get; set; }
    }
}
