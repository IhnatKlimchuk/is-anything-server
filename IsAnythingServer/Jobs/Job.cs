using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Jobs
{
    public abstract class Job : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan? _executionTimeout;

        protected readonly ILogger Logger;

        public Job(
            IServiceProvider serviceProvider,
            IOptions<JobSettings> settings,
            ILogger logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _executionTimeout = settings?.Value?.ExecutionTimeout;

            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected abstract Task ExecuteAsync(IServiceScope serviceScope, CancellationToken cancellationToken = default);

        private async Task ExecuteWithTimeoutAsync(IServiceScope serviceScope, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            using CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource(timeout);
            using CancellationTokenSource executionCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token);
            try
            {
                await ExecuteAsync(serviceScope, executionCancellationTokenSource.Token);
            }
            finally
            {
                if (timeoutCancellationTokenSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    Logger.LogWarning("Job was cancelled due to timeout.");
                }
            }
        }

        protected async Task TriggerExecutionAsync(CancellationToken cancellationToken = default)
        {
            DateTime executionStartDateTime = DateTime.UtcNow;
            using var scope = _serviceProvider.CreateScope();
            if (_executionTimeout.HasValue)
            {
                await ExecuteWithTimeoutAsync(scope, _executionTimeout.Value, cancellationToken);
            }
            else
            {
                await ExecuteAsync(scope, cancellationToken);
            }
            Logger.LogInformation($"Job execution took {DateTime.UtcNow - executionStartDateTime}");
        }

        public abstract Task StartAsync(CancellationToken cancellationToken = default);

        public abstract Task StopAsync(CancellationToken cancellationToken = default);

        public virtual void Dispose()
        {
            //nothing to do here...
        }
    }
}
