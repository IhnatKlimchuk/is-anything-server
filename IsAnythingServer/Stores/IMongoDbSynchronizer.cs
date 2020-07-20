using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Stores
{
    public interface IMongoDbSynchronizer
    {
        Task SynchronizeAsync(CancellationToken cancellationToken = default);
    }
}
