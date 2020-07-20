using MongoDB.Bson.Serialization.Attributes;

namespace IsAnythingServer.Stores.Records
{
    public class RecordMongoDbStatistic
    {
        [BsonElement("trueCounter")]
        public long TrueCounter { get; set; }
        [BsonElement("falseCounter")]
        public long FalseCounter { get; set; }
    }
}
