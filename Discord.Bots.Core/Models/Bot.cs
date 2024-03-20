namespace Discord.Bots.Core.Models;

public sealed record Bot
{
    public required string Token { get; set; }
    public ulong OwnerID { get; set; }
    public Dictionary<ulong, Server> Servers { get; set; } = new();
}
