using System;
using System.Collections.Generic;

namespace The_Wanderers_Helper.Config
{
    public class ServerConfig
    {
        public ServerConfig()
        {
            SortRoles = new List<ulong>();
        }

        public string Name { get; set; }
        public string Prefix { get; set; }
        public ulong? ModChannel { get; set; }
        public bool EnableBirthdayChecking { get; set; }
        public List<ulong> SortRoles { get; set; }
        public List<(ulong, DateTime)> Birthdays { get; set; }
    }
}
