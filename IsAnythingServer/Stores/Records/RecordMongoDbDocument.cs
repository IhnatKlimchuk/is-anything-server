using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace IsAnythingServer.Stores.Records
{
    public class RecordMongoDbDocument
    {
        [BsonId]
        public RecordMongoDbKey Key { get; set; }
        public long TrueDailyCounter { get; set; }
        public long FalseDailyCounter { get; set; }
        public long TrueTotalCounter { get; set; }
        public long FalseTotalCounter { get; set; }
        public bool LastValue { get; set; }
        public Dictionary<DateTime, RecordMongoDbStatistic> Statistics { get; set; }

        public const string CollectionName = "Record";
    }
}
