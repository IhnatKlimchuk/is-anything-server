using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace IsAnythingServer.Stores.Records
{
    public class RecordMongoDbDocument
    {
        [BsonId]
        public RecordMongoDbKey Key { get; set; }
        [BsonElement("trueTotalCounter")]
        public long TrueTotalCounter { get; set; }
        [BsonElement("falseTotalCounter")]
        public long FalseTotalCounter { get; set; }
        [BsonElement("lastValue")]
        public bool LastValue { get; set; }
        [BsonElement("statistics")]
        public Dictionary<string, RecordMongoDbStatistic> Statistics { get; set; }

        public const string CollectionName = "Record";
    }
}
