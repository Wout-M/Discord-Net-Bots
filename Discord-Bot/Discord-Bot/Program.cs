using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.Events;
using Discord_Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Net.Http;

namespace Discord_Bot
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
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 100
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async,
            }))
            //Events
            .AddSingleton<MessageEvents>()
            .AddSingleton<GuildEvents>()
            .AddSingleton<InteractionEvents>()
            //Services
            .AddSingleton<ConfigService>()
            .AddSingleton<LoggingService>()
            .AddSingleton<StartupService>()
            .AddSingleton<EventService>()
            //HttpClientFactory
            .AddHttpClient();

        }
    }
}
