using Api.Jobs;
using Quartz;

internal static class QuartzServiceCollectionExtensions
{
    internal static IServiceCollection AddCronServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddQuartz(qc =>
        {
            var emailKey = new JobKey(nameof(SendReminderEmailJob));
            qc.AddJob<SendReminderEmailJob>(jc => jc.WithIdentity(emailKey));
            qc.AddTrigger(tc => tc
                .ForJob(emailKey)
                .WithCronSchedule("0 0 9 1/7 * ?")
            );

            // var syncKey = new JobKey(nameof(IcuSyncJob));
            // qc.AddJob<IcuSyncJob>(jc => jc.WithIdentity(syncKey));
            // qc.AddTrigger(tc => tc
            //     .ForJob(syncKey)
            //     .WithCronSchedule("0 0 0 * * ?")
            // );
        });

        serviceCollection.AddQuartzHostedService(so => so.WaitForJobsToComplete = true);

        return serviceCollection;
    }
}