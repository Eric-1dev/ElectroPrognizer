using ElectroPrognizer.SchedulerServices.Jobs;
using Quartz;

namespace ElectroPrognizer.SchedulerServices.Extensions;

public static class QuartzExtensions
{
    public static void RegisterJobs(this IServiceCollectionQuartzConfigurator quartzConfigurator)
    {
        quartzConfigurator.RegisterJob<ReceiveNewFilesFromMailJob>("0 */10 * ? * *");
    }

    private static void RegisterJob<T>(this IServiceCollectionQuartzConfigurator quartzConfigurator, string cronExpression) where T : IJob
    {
        var jobKey = new JobKey(typeof(T) + "Key");
        var jobTrigger = typeof(T) + "Trigger";

        quartzConfigurator.AddJob<T>(option => option.WithIdentity(jobKey));

        quartzConfigurator.AddTrigger(option =>
        {
            option.ForJob(jobKey)
                .WithIdentity(jobTrigger)
                .WithCronSchedule(cronExpression);
        });
    }
}
