using MongoDB.Bson.Serialization.Attributes;

namespace IsAnythingServer.Stores.Records
{
    public class RecordMongoDbKey
    {
        [BsonElement("subject")]
        public string Subject { get; set; }
        [BsonElement("predicate")]
        public string Predicate { get; set; }
    }
}
