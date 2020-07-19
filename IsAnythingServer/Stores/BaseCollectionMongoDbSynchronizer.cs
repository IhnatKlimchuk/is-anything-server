using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Stores
{
    public abstract class BaseCollectionMongoDbSynchronizer<TDocument> : IMongoDbSynchronizer
    {
        protected IMongoCollection<TDocument> Collection;
        public BaseCollectionMongoDbSynchronizer(IMongoCollection<TDocument> collection)
        {
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public abstract Task SynchronizeAsync(CancellationToken cancellationToken = default);
    }
}
