using Discord.Bots.Core;
using Discord.Bots.Core.Modules;

namespace Discord.Bots.KGB;

internal class Program
{
    private static readonly Type[] Modules =
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
        Startup.RunAsync().GetAwaiter().GetResult();
    }
}
