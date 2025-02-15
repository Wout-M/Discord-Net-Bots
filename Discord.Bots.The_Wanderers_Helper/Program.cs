using Discord.Bots.Core;
using Discord.Bots.Core.Config;
using Discord.Bots.Core.Modules;

namespace Discord.Bots.The_Wanderers_Helper;

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
    ];

    static void Main(string[] args)
    {
        Config.UseBirthdayChecking = true;
        Config.ActivityType = ActivityType.Watching;
        Config.ActivityMessage = "to see if everyone behaves";
        Config.SortMenuTitle = "Choose a guild to get sorted in";
        Config.SortMenuDescription = "Click on the button with your guild";

        Startup.RunAsync(_modules).GetAwaiter().GetResult();
    }
}
