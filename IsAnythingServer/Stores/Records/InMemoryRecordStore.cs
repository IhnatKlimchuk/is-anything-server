using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Stores.Records
{
    public sealed class InMemoryRecordStore : IRecordStore
    {
        private readonly ConcurrentDictionary<(string subject, string predicate), bool> _storage;

        public InMemoryRecordStore()
        {
            _storage = new ConcurrentDictionary<(string subject, string predicate), bool>();
        }

        public Task<bool> CreateOrUpdateRecordAsync(string subject, string predicate, bool value, CancellationToken cancellationToken = default)
        {
            _storage.AddOrUpdate((subject, predicate), value, (key, oldValue) => value);
            return Task.FromResult(value);
        }

        public Task<bool?> GetRecordAsync(string subject, string predicate, CancellationToken cancellationToken = default)
        {
            if (_storage.TryGetValue((subject, predicate), out bool result))
            {
                return Task.FromResult<bool?>(result);
            }
            return null;
        }
    }
}
