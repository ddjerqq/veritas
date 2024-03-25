using System.ComponentModel;
using Infrastructure;
using Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.Hosting;
using Quartz;

[assembly: HostingStartup(typeof(ConfigureBackgroundJobs))]

namespace Infrastructure;

[EditorBrowsable(EditorBrowsableState.Never)]
public class ConfigureBackgroundJobs : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddQuartz(config =>
            {
                config
                    .AddJob<ProcessVotesBackgroundJob>(ProcessVotesBackgroundJob.Key, job => { job.StoreDurably(); })
                    .AddTrigger(trigger => trigger
                        .ForJob(ProcessVotesBackgroundJob.Key)
                        .WithSimpleSchedule(schedule => schedule
                            .WithInterval(TimeSpan.FromSeconds(5))
                            .RepeatForever()));

                config
                    .AddJob<ProcessOutboxMessagesBackgroundJob>(ProcessOutboxMessagesBackgroundJob.Key, job => { job.StoreDurably(); })
                    .AddTrigger(trigger => trigger
                        .ForJob(ProcessOutboxMessagesBackgroundJob.Key)
                        .WithSimpleSchedule(schedule => schedule
                            .WithInterval(TimeSpan.FromSeconds(10))
                            .RepeatForever()));

                config.UseInMemoryStore();
            });

            services.AddQuartzHostedService();
        });
    }
}
