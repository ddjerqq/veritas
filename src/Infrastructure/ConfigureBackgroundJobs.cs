using Infrastructure;
using Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.Hosting;
using Quartz;

[assembly: HostingStartup(typeof(ConfigureBackgroundJobs))]

namespace Infrastructure;

public class ConfigureBackgroundJobs : IHostingStartup
{
    private static bool _configured;

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        builder.ConfigureServices(services =>
        {
            services.AddQuartz(config =>
            {
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
