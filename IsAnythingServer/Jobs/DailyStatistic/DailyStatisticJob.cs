using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Jobs.DailyStatistic
{
    public class DailyStatisticJob : CronJob
    {
        public DailyStatisticJob(
            IServiceProvider serviceProvider,
            IOptions<DailyStatisticJobSettings> settings,
            ILogger<DailyStatisticJob> logger) : base(serviceProvider, settings, logger)
        {
        }

        protected override Task ExecuteAsync(IServiceScope serviceScope, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
