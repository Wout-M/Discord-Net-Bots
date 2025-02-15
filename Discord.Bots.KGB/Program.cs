using Discord.Bots.Core;
using Discord.Bots.Core.Config;
using Discord.Bots.Core.Modules;

namespace Discord.Bots.KGB;

internal class Program
{
    private static readonly Type[] _modules =
    [
        typeof(AdminModule),
        typeof(BirthdayModule),
        typeof(ButtonModule),
        typeof(FunModule),
        typeof(UtilityModule),
        typeof(WordsModule),
    ];

    static void Main(string[] args)
    {
        Config.UseBirthdayChecking = true;
        Config.UseWordsChecking = true;
        Config.UseMessageLogging = true;
        Config.ActivityType = ActivityType.Watching;
        Config.ActivityMessage = "to see if everyone behaves";

        Startup.RunAsync(_modules).GetAwaiter().GetResult();
    }
}
