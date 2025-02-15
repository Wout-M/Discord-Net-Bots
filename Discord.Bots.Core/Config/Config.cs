namespace Discord.Bots.Core.Config;

public static class Config
{
    public static bool UseBirthdayChecking {  get; set; }
    public static bool ShowAge {  get; set; }

    public static bool UseWordsChecking { get; set; }
    public static bool UseMessageLogging { get; set; }

    public static string ActivityMessage { get; set; } = string.Empty;
    public static ActivityType ActivityType { get; set; }

    public static bool AllowMultipleSortRoles { get; set; }
    public static bool AllowSortRoleRemoval { get; set; }
    public static string SortMenuTitle {  get; set; } = string.Empty;
    public static string SortMenuDescription { get; set; } = string.Empty;
}
