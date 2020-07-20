using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Jobs
{
    public class JobsScheduler : IHostedService, IDisposable
    {
        private IList<IJob> _jobs;
        private readonly IServiceProvider _services;
        private readonly ILogger<JobsScheduler> _logger;

        public JobsScheduler(
            IServiceProvider services,
            ILogger<JobsScheduler> logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_jobs != null)
            {
                throw new InvalidOperationException($"Duplicate called detected.");
            }

            _jobs = _services.GetServices<IJob>().ToList();
            _logger.LogInformation($"Scheduling {_jobs.Count} jobs one by one...");
            foreach (var job in _jobs)
            {
                _logger.LogInformation($"Scheduling {job.GetType().Name}.");
                await job.StartAsync(cancellationToken);
                _logger.LogInformation($"{job.GetType().Name} is scheduled.");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Unscheduling {_jobs.Count} jobs...");
            var tasksToWait = _jobs
                .Select(job => job
                    .StopAsync(cancellationToken)
                    .ContinueWith(task => _logger.LogInformation($"{job.GetType().Name} is unscheduled.")))
                .ToList();
            await Task.WhenAll(tasksToWait);
            _logger.LogInformation($"Unscheduling completed...");
        }

        public void Dispose()
        {
            if (_jobs != null)
            {
                foreach (var job in _jobs)
                {
                    job.Dispose();
                }
            }
        }
    }
}
