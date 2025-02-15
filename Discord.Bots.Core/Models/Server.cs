namespace Discord.Bots.Core.Models;

public sealed record Server
{
    public required string Name { get; set; }
    public ulong? ModChannel { get; set; }
    public ulong? LogChannel { get; set; }
    public bool EnableBirthdayChecking { get; set; }
    public List<(string word, List<(ulong userId, int count)> scores)> Words { get; set; } = [];
    public List<(ulong userId, DateTime birthday)> Birthdays { get; set; } = [];
    public List<ulong> SortRoles { get; set; } = [];
}
