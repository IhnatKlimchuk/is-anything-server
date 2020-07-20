using IsAnythingServer.Jobs.DailyStatistic;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        public async Task<bool> CreateOrUpdateRecordAsync(string subject, string predicate, bool value, CancellationToken cancellationToken = default)
        {
            long trueIncrement = value ? 1 : 0;
            long falseIncrement = value ? 0 : 1;
            var result = await _recordCollection.FindOneAndUpdateAsync(
                filter: new FilterDefinitionBuilder<RecordMongoDbDocument>().And(
                    new FilterDefinitionBuilder<RecordMongoDbDocument>().Eq(document => document.Key.Subject, subject),
                    new FilterDefinitionBuilder<RecordMongoDbDocument>().Eq(document => document.Key.Predicate, predicate)),
                update: Builders<RecordMongoDbDocument>.Update.Combine(
                    Builders<RecordMongoDbDocument>.Update.Set(document => document.LastValue, value),
                    Builders<RecordMongoDbDocument>.Update.Inc(document => document.TrueDailyCounter, trueIncrement),
                    Builders<RecordMongoDbDocument>.Update.Inc(document => document.TrueTotalCounter, trueIncrement),
                    Builders<RecordMongoDbDocument>.Update.Inc(document => document.FalseDailyCounter, falseIncrement),
                    Builders<RecordMongoDbDocument>.Update.Inc(document => document.FalseTotalCounter, falseIncrement)),
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

        public async IAsyncEnumerable<Record> GetActiveRecordsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var cursor = await _recordCollection.FindAsync(
                filter: new FilterDefinitionBuilder<RecordMongoDbDocument>().Or(
                    new FilterDefinitionBuilder<RecordMongoDbDocument>().Gt(document => document.TrueDailyCounter, 0),
                    new FilterDefinitionBuilder<RecordMongoDbDocument>().Gt(document => document.FalseDailyCounter, 0)),
                options: new FindOptions<RecordMongoDbDocument>()
                {
                    Projection = new ProjectionDefinitionBuilder<RecordMongoDbDocument>().Exclude(document => document.Statistics)
                },
                cancellationToken: cancellationToken);

            while (await cursor.MoveNextAsync(cancellationToken))
            {
                foreach (var document in cursor.Current)
                {
                    yield return ConvertToRecord(document);
                }
            }
        }

        public async Task UpdateDailyStatisticAsync(string subject, string predicate, string date, long trueDailyCounter, long falseDailyCounter, CancellationToken cancellationToken = default)
        {
            var statistic = new RecordMongoDbStatistic
            {
                TrueCounter = trueDailyCounter,
                FalseCounter = falseDailyCounter
            };
            var result = await _recordCollection.UpdateOneAsync(
                filter: new FilterDefinitionBuilder<RecordMongoDbDocument>().And(
                    new FilterDefinitionBuilder<RecordMongoDbDocument>().Eq(document => document.Key.Subject, subject),
                    new FilterDefinitionBuilder<RecordMongoDbDocument>().Eq(document => document.Key.Predicate, predicate)),
                update: Builders<RecordMongoDbDocument>.Update.Combine(
                    Builders<RecordMongoDbDocument>.Update.Inc(document => document.TrueDailyCounter, -trueDailyCounter),
                    Builders<RecordMongoDbDocument>.Update.Inc(document => document.FalseDailyCounter, -falseDailyCounter),
                    Builders<RecordMongoDbDocument>.Update.Set($"statistics.{date}", statistic)),
                options: null,
                cancellationToken: cancellationToken);
        }

        private static Record ConvertToRecord(RecordMongoDbDocument document)
        {
            return new Record
            {
                Subject = document.Key.Subject,
                Predicate = document.Key.Predicate,
                FalseDailyCounter = document.FalseDailyCounter,
                TrueDailyCounter = document.TrueDailyCounter,
                FalseTotalCounter = document.FalseTotalCounter,
                TrueTotalCounter = document.TrueTotalCounter,
                LastValue = document.LastValue
            };
        }
    }
}
