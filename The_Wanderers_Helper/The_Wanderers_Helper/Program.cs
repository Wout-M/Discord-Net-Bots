using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using The_Wanderers_Helper.Events;
using The_Wanderers_Helper.Jobs;
using The_Wanderers_Helper.Services;

namespace The_Wanderers_Helper
{
    class Program
    {
        public static void Main(string[] args)
        => new Program().RunAsync().GetAwaiter().GetResult();

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
            //Client
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 100,
                AlwaysDownloadUsers = true
            }))
            .AddSingleton(s => new InteractionService(s.GetRequiredService<DiscordSocketClient>()))
            //Events
            .AddSingleton<GuildEvent>()
            .AddSingleton<InteractionEvent>()
            .AddSingleton<ReadyEvent>()
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
