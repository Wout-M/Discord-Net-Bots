using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord_Bot.Events;
using Discord_Bot.Jobs;
using Discord_Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Discord_Bot
{
    class Program
    {
        public static void Main(string[] args)  => new Program().RunAsync().GetAwaiter().GetResult();

        public async Task RunAsync()
        {
            var services = new ServiceCollection();
            RegisterServices(services);

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<LoggingService>();
            provider.GetRequiredService<ConfigService>();

            await provider.GetRequiredService<StartupService>().StartAsync();
            await Task.Delay(-1);
        }

        private void RegisterServices(IServiceCollection services)
        {
            services
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 100
            }))
           .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
           //Events
           .AddSingleton<GuildEvent>()
           .AddSingleton<ReadyEvent>()
           .AddSingleton<MessageEvent>()
           .AddSingleton<LoggedInEvent>()
           .AddSingleton<InteractionEvent>()
           //Services
           .AddSingleton<ConfigService>()
           .AddSingleton<LoggingService>()
           .AddSingleton<StartupService>()
           .AddSingleton<EventService>()
           //HttpClientFactory
           .AddHttpClient()
           //Quartz
           .AddScoped<BirthdayJob>()
           .AddQuartz();
            
        }
    }
}
