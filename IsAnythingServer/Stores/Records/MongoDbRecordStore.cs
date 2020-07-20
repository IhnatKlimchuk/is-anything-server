using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IsAnythingServer.Stores.Records
{
    public class MongoDbRecordStore : IRecordStore
    {
        private readonly IMongoCollection<RecordMongoDbDocument> _recordCollection;
        private readonly ILogger<MongoDbRecordStore> _logger;

        public MongoDbRecordStore(
            IMongoCollection<RecordMongoDbDocument> recordCollection,
            ILogger<MongoDbRecordStore> logger)
        {
            _recordCollection = recordCollection ?? throw new ArgumentNullException(nameof(recordCollection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> CreateOrUpdateRecordAsync(string subject, string predicate, string date, bool value, CancellationToken cancellationToken = default)
        {
            long trueIncrement = value ? 1 : 0;
            long falseIncrement = value ? 0 : 1;
            var result = await _recordCollection.FindOneAndUpdateAsync(
                filter: new FilterDefinitionBuilder<RecordMongoDbDocument>().And(
                    new FilterDefinitionBuilder<RecordMongoDbDocument>().Eq(document => document.Key.Subject, subject),
                    new FilterDefinitionBuilder<RecordMongoDbDocument>().Eq(document => document.Key.Predicate, predicate)),
                update: Builders<RecordMongoDbDocument>.Update.Combine(
                    Builders<RecordMongoDbDocument>.Update.Set(document => document.LastValue, value),
                    Builders<RecordMongoDbDocument>.Update.Inc(document => document.TrueTotalCounter, trueIncrement),
                    Builders<RecordMongoDbDocument>.Update.Inc(document => document.FalseTotalCounter, falseIncrement),
                    Builders<RecordMongoDbDocument>.Update.Inc($"statistics.{date}.trueCounter", trueIncrement),
                    Builders<RecordMongoDbDocument>.Update.Inc($"statistics.{date}.falseCounter", falseIncrement)),
                options: new FindOneAndUpdateOptions<RecordMongoDbDocument> 
                { 
                    IsUpsert = true, 
                    ReturnDocument = ReturnDocument.After,
                    Projection = new ProjectionDefinitionBuilder<RecordMongoDbDocument>().Exclude(document => document.Statistics)
                },
                cancellationToken: cancellationToken);
            return result.LastValue;
        }

        public async Task<bool?> GetRecordAsync(string subject, string predicate, CancellationToken cancellationToken = default)
        {
            var cursor = await _recordCollection.FindAsync(
                filter: new FilterDefinitionBuilder<RecordMongoDbDocument>().And(
                    new FilterDefinitionBuilder<RecordMongoDbDocument>().Eq(document => document.Key.Subject, subject),
                    new FilterDefinitionBuilder<RecordMongoDbDocument>().Eq(document => document.Key.Predicate, predicate)),
                options: new FindOptions<RecordMongoDbDocument>() 
                { 
                    Projection = new ProjectionDefinitionBuilder<RecordMongoDbDocument>().Exclude(document => document.Statistics)
                },
                cancellationToken: cancellationToken);
            var result = await cursor.FirstOrDefaultAsync(cancellationToken);
            return result?.LastValue;
        }
    }
}
