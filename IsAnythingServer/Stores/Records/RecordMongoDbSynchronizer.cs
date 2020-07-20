using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Stores.Records
{
    public class RecordMongoDbSynchronizer : BaseCollectionMongoDbSynchronizer<RecordMongoDbDocument>
    {
        public RecordMongoDbSynchronizer(IMongoCollection<RecordMongoDbDocument> mongoCollection) : base(mongoCollection)
        {
        }

        public async override Task SynchronizeAsync(CancellationToken cancellationToken = default)
        {
            await Collection.Indexes.CreateOneAsync(
                model: new CreateIndexModel<RecordMongoDbDocument>(
                    keys: new IndexKeysDefinitionBuilder<RecordMongoDbDocument>()
                        .Text(document => document.Key.Subject)
                        .Text(document => document.Key.Predicate),
                    options: new CreateIndexOptions
                    {
                        Name = "subject-predicate_text-index",
                    }),
                options: null,
                cancellationToken: cancellationToken);
        }
    }
}
