namespace Discord.Bots.Core.Models;

public class Bot
{
    public Bot()
    {
        Servers = new();
    }

    public string Token { get; set; }
    public ulong OwnerID { get; set; }
    public Dictionary<ulong, Server> Servers { get; set; }
}
