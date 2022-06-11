using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Jobs
{
    public static class QuartzExtensions
    {

        public static async void AddQuartz(this IServiceCollection services)
        {

            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();

            var jobFactory = new JobFactory(services.BuildServiceProvider());
            scheduler.JobFactory = jobFactory;
            await scheduler.Start();


            IJobDetail job = JobBuilder.Create<BirthdayJob>()
                .StoreDurably()
                .WithIdentity("job1", "group1")
                .WithDescription("Birthday checking")
                .Build();

            await scheduler.AddJob(job, true);

            services.AddSingleton(scheduler);
        }
    }
}
