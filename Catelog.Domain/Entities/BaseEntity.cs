using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Catelog.Domain.Entities
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

    }
}
