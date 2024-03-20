using Quartz;
using Quartz.Spi;

namespace Discord.Bots.Core.Jobs;

public class JobFactory : IJobFactory
{
    protected readonly IServiceProvider Container;

    public JobFactory(IServiceProvider container)
    {
        Container = container;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        if (Container.GetService(bundle.JobDetail.JobType) is not IJob job) { throw new Exception("Job not found"); }
        return job;
    }

    public void ReturnJob(IJob job)
    {
        (job as IDisposable)?.Dispose();
    }
}
