using MongoDB.Bson.Serialization.Attributes;

namespace Catelog.Domain.Entities
{
    public class ProductType : BaseEntity
    {
        [BsonElement("Name")]
        public string Name { get; set; }
    }
}
