using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCrontab;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Jobs
{
    public abstract class CronJob : Job, IDisposable
    {
        private readonly bool _isExecutedOnStart;
        private readonly bool _isActive;
        private readonly CrontabSchedule _crontabSchedule;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Task _runningTask;

        public CronJob(
            IServiceProvider serviceProvider,
            IOptions<CronJobSettings> settings,
            ILogger logger) : base(serviceProvider, settings, logger)
        {
            _isExecutedOnStart = settings?.Value?.IsExecutedOnStart ?? throw new ArgumentNullException(nameof(settings.Value.IsExecutedOnStart));
            _isActive = settings?.Value?.IsActive ?? throw new ArgumentNullException(nameof(settings.Value.IsActive));
            _crontabSchedule = CrontabSchedule.Parse(settings?.Value?.Cron ?? throw new ArgumentNullException(nameof(settings.Value.Cron)));
        }

        public override Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_isActive)
            {
                _runningTask = Task.Run(RunCronTaskAsync, cancellationToken);
            }
            else
            {
                _runningTask = Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource.Cancel();
            try
            {
                await _runningTask;
            }
            catch (Exception exception)
            {
                Logger.LogWarning(exception, "Error during stopping job.");
            }


        }

        private async Task RunCronTaskAsync()
        {
            TimeSpan timeTillNextRun = _isExecutedOnStart ? TimeSpan.Zero : CalculateTimeToWait();
            do
            {
                try
                {
                    Logger.LogInformation($"Waiting {timeTillNextRun} for next run...");
                    await Task.Delay(timeTillNextRun, _cancellationTokenSource.Token);
                    Logger.LogInformation("Trigger job execution...");
                    await TriggerExecutionAsync(_cancellationTokenSource.Token);
                    timeTillNextRun = CalculateTimeToWait();
                }
                catch (Exception exception)
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        Logger.LogWarning(exception, "Job was cancelled.");
                    }
                    else
                    {
                        Logger.LogError(exception, "Job execution has ended with error.");
                    }
                }

            } while (!_cancellationTokenSource.Token.IsCancellationRequested);
        }

        private TimeSpan CalculateTimeToWait()
        {
            var currentDateTime = DateTime.UtcNow;
            var nextExecutionExpectedDate = _crontabSchedule.GetNextOccurrence(currentDateTime);
            return nextExecutionExpectedDate - currentDateTime;
        }

        public override void Dispose()
        {
            try
            {
                _runningTask?.Dispose();
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                base.Dispose();
            }
        }
    }
}
