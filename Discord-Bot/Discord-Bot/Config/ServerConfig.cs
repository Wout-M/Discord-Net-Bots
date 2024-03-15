using System;
using System.Collections.Generic;

namespace Discord_Bot.Config;
public class ServerConfig
{
    public ServerConfig()
    {
        SortRoles = new List<ulong>();
        Birthdays = new List<(ulong, DateTime)>();
    }

    public string Name { get; set; }
    public string Prefix { get; set; }
    public ulong? ModChannelID { get; set; }
    public ulong? LogChannelID { get; set; }
    public bool EnableBirthdayChecking { get; set; }
    public List<ulong> SortRoles { get; set; }
    public List<(ulong, DateTime)> Birthdays { get; set; }
}
