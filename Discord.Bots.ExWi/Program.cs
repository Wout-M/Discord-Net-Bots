using Discord.Bots.Core;
using Discord.Bots.Core.Config;
using Discord.Bots.Core.Modules;

namespace Discord.Bots.ExWi;

internal class Program
{
    private static readonly Type[] _modules =
    [
        typeof(AdminModule),
        typeof(ButtonModule),
        typeof(SortModule),
        typeof(UtilityModule),
    ];

    static void Main(string[] args)
    {
        Config.UseMessageLogging = true;
        Config.ActivityType = ActivityType.Watching;
        Config.ActivityMessage = "to see if everyone behaves";
        Config.SortMenuTitle = "Choose a role to get sorted in";
        Config.SortMenuDescription = "Klick auf den Button mit deiner Rolle. Möchtest du eine bereits bestehende Rolle entfernen, klicke nochmal auf den Button.";
        Config.AllowSortRoleRemoval = true;
        Config.AllowMultipleSortRoles = true;

        Startup.RunAsync(_modules).GetAwaiter().GetResult();
    }
}
