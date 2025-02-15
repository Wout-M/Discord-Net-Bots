using Discord.Bots.Core;
using Discord.Bots.Core.Config;
using Discord.Bots.Core.Modules;

namespace Discord.Bots.Test;

internal class Program
{
    private static readonly Type[] _modules =
    [
        typeof(AdminModule),
        typeof(BirthdayModule),
        typeof(ButtonModule),
        typeof(FunModule),
        typeof(SortModule),
        typeof(UtilityModule),
        typeof(WordsModule)
    ];

    static void Main(string[] args)
    {
        Config.UseBirthdayChecking = true;
        Config.UseWordsChecking = true;
        Config.UseMessageLogging = true;
        Config.ActivityType = ActivityType.Watching;
        Config.ActivityMessage = "to see if everyone behaves";
        Config.SortMenuTitle = "Choose a guild to get sorted in";
        Config.SortMenuDescription = "Click on the button with your guild";
        Config.AllowSortRoleRemoval = true;
        Config.AllowMultipleSortRoles = true;

        Startup.RunAsync(_modules).GetAwaiter().GetResult();
    }
}
