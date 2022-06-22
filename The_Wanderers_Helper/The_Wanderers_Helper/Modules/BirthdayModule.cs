using Discord;
using Discord.Interactions;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using The_Wanderers_Helper.Services;

namespace The_Wanderers_Helper.Modules
{
    [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    [Group("birthday", "Birthday configurations")]
    public class BirthdayModule : InteractionModuleBase<InteractionContext>
    {
        private readonly ConfigService _configService;
        private readonly IScheduler _scheduler;

        public BirthdayModule(ConfigService configService, IScheduler scheduler)
        {
            _configService = configService;
            _scheduler = scheduler;
        }

        [SlashCommand("add", "Add a new birthday")]
        public async Task Add([Summary("User", "The birthday person")] IUser user, DateTime birthday)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);

            if (!config.Birthdays.Any(x => x.userId == user.Id))
            {
                config.Birthdays.Add(new(user.Id, birthday));
                await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
                await RespondAsync($"Successfully added `{user.Username}` ({birthday:dd/MM/yyyy}) to the birthday list");
            }
            else
            {
                var bday = config.Birthdays.First(x => x.userId == user.Id);
                config.Birthdays.Remove(bday);
                bday.Item2 = birthday;
                config.Birthdays.Add(bday);
                await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
                await RespondAsync($"Successfully updated `{user.Username}` ({birthday:dd/MM/yyyy})");
            }
        }

        [SlashCommand("remove", "Remove a birthday")]
        public async Task Remove([Summary("User", "The birthday person")] IUser user)
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);

            if (!config.Birthdays.Any(x => x.userId == user.Id))
            {
                await RespondAsync($"There is no birthday for `{user.Username}` in the birthday list");
            }
            else
            {
                var bday = config.Birthdays.First(x => x.userId == user.Id);
                config.Birthdays.Remove(bday);
                await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
                await RespondAsync($"Successfully removed `{user.Username}`");
            }
        }

        [SlashCommand("list", "List all the birthdays")]
        public async Task List()
        {
            await DeferAsync();

            var config = await _configService.GetServerConfig(Context.Guild.Id);
            var birthdays = await Task.WhenAll(config.Birthdays
                .OrderBy(x => x.birthday.Month)
                .ThenBy(x => x.birthday.Day)
                .Select(async x =>
                {
                    var username = string.Empty;
                    var guildUser = await Context.Guild.GetUserAsync(x.userId);
                    if (guildUser != null)
                    {
                        username = string.IsNullOrEmpty(guildUser.Nickname) ? guildUser.Username : guildUser.Nickname;
                    }
                    else
                    {
                        var user = await Context.Client.GetUserAsync(x.userId);
                        username = string.IsNullOrEmpty(user?.Username) ? "No user found" : user.Username;
                    }

                    return $"`{x.birthday:dd/MM}`: {username}";
                }));

            string text = birthdays.Any()
                ? string.Join(Environment.NewLine, birthdays)
                : "No birthdays";

            var embedBuilder = new EmbedBuilder()
                .WithDescription("Here's a list of everyones birthday")
                .WithColor(Color.DarkPurple)
                .AddField("Birthdays", text);

            await RespondAsync(embed: embedBuilder.Build());
        }

        [SlashCommand("start", "Start birthday checking")]
        public async Task Start()
        {
            var jobKey = new JobKey("job1", "group1");
            var trigger = TriggerBuilder.Create()
                .ForJob(jobKey)
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            var triggers = await _scheduler.GetTriggersOfJob(jobKey);
            if (!triggers.Any())
            {
                await _scheduler.ScheduleJob(trigger);
                await RespondAsync("Started checking for birthdays");
            }
            else
            {
                await RespondAsync("Birthday checking is already running");
            }
        }


        [SlashCommand("stop", "Stop birthday checking")]
        public async Task Stop()
        {

            var success = await _scheduler.UnscheduleJob(new TriggerKey("trigger1", "group1"));
            var text = success
                ? "Stopped checking for birthdays"
                : "Something went wrong while stopping the birthday checking. Maybe no check was running.";
            await RespondAsync(text);
        }

        [SlashCommand("enable", "Enable birthday checking")]
        public async Task Enable()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            if (!config.EnableBirthdayChecking)
            {
                config.EnableBirthdayChecking = true;
                await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
            }

            await RespondAsync("Enabled birthday checking for this server");
        }

        [SlashCommand("disable", "Disable birthday checking")]
        public async Task Disable()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            if (config.EnableBirthdayChecking)
            {
                config.EnableBirthdayChecking = false;
                await _configService.AddOrUpdateServerConfig(Context.Guild.Id, config);
            }

            await RespondAsync("Disabled birthday checking for this server");
        }

        [SlashCommand("check", "Check birthday checking")]
        public async Task Check()
        {
            var config = await _configService.GetServerConfig(Context.Guild.Id);
            var jobKey = new JobKey("job1", "group1");
            var triggers = await _scheduler.GetTriggersOfJob(jobKey);

            string text = triggers.Any()
                ? "Birthday checking is running"
                : "Nothing is running";

            var embedBuilder = new EmbedBuilder()
                .WithDescription("Is birthday checking running?")
                .WithColor(Color.DarkTeal)
                .AddField("Enabled (for this server)", config.EnableBirthdayChecking ? "Enabled" : "Disabled")
                .AddField("Running (for all servers)", text);

            await RespondAsync(embed: embedBuilder.Build());
        }
    }
}
