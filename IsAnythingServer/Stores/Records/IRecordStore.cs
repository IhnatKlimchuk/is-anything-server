using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Stores.Records
{
    public interface IRecordStore
    {
        Task<bool?> GetRecordAsync(string subject, string predicate, CancellationToken cancellationToken = default);
        Task<bool> CreateOrUpdateRecordAsync(string subject, string predicate, string date, bool value, CancellationToken cancellationToken = default);
    }
}
