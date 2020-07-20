using System;
using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Jobs
{
    public interface IJob : IDisposable
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
