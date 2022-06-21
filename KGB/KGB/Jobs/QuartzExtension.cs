using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;

namespace KGB.Jobs
{
    public static class QuartzExtension
    {
        public static async void AddQuartz(this IServiceCollection services)
        {

            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();

            var jobFactory = new JobFactory(services.BuildServiceProvider());
            scheduler.JobFactory = jobFactory;
            await scheduler.Start();

            var jobKey = new JobKey("job1", "group1");
            var job = JobBuilder.Create<BirthdayJob>()
                .StoreDurably()
                .WithIdentity(jobKey)
                .WithDescription("Birthday checking")
                .Build();

            await scheduler.AddJob(job, true);

            services.AddSingleton(scheduler);
        }
    }
}
