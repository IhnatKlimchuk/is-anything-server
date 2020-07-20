using IsAnythingServer.Jobs.DailyStatistic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Stores.Records
{
    public interface IRecordStore
    {
        Task<bool?> GetRecordAsync(string subject, string predicate, CancellationToken cancellationToken = default);
        Task<bool> CreateOrUpdateRecordAsync(string subject, string predicate, bool value, CancellationToken cancellationToken = default);
        IAsyncEnumerable<Record> GetActiveRecordsAsync(CancellationToken cancellationToken = default);
        Task UpdateDailyStatisticAsync(string subject, string predicate, string date, long trueDailyCounter, long falseDailyCounter, CancellationToken cancellationToken = default);
    }
}
