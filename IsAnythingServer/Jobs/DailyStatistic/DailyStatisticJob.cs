using IsAnythingServer.Stores.Records;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
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

        protected override async Task ExecuteAsync(IServiceScope serviceScope, CancellationToken cancellationToken = default)
        {
            var recordStore = serviceScope.ServiceProvider.GetRequiredService<IRecordStore>();
            var date = DateTime.UtcNow.Date.ToString("dd-MM-yyyy", DateTimeFormatInfo.InvariantInfo);

            var records = recordStore.GetActiveRecordsAsync(cancellationToken);
            await foreach (var record in records)
            {
                try
                {
                    await recordStore.UpdateDailyStatisticAsync(
                    subject: record.Subject,
                    predicate: record.Predicate,
                    date: date,
                    trueDailyCounter: record.TrueDailyCounter,
                    falseDailyCounter: record.FalseDailyCounter,
                    cancellationToken: cancellationToken);
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, $"Failed to update statistic for Subject:{record.Subject} Predicate:{record.Predicate}.");
                }
            }
        }
    }
}
