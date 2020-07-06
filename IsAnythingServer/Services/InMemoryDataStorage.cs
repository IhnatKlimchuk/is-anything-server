using System.Collections.Concurrent;

namespace IsAnythingServer.Services
{
    public sealed class InMemoryDataStorage : IDataStorage
    {
        private ConcurrentDictionary<(string subject, string predicate), bool> _storage = new ConcurrentDictionary<(string subject, string predicate), bool>();
        public bool? ReadRecord(string subject, string predicate)
        {
            if (_storage.TryGetValue((subject, predicate), out bool result))
            {
                return result;
            }
            return null;
        }

        public bool WriteRecord(string subject, string predicate, bool value)
        {
            return _storage.AddOrUpdate((subject, predicate), value, (key, oldValue) => value);
        }
    }
}
