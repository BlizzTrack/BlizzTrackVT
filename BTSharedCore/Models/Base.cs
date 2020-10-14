using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BTSharedCore.Models
{
    public class Base
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public ObjectId? Id { get; set; } = ObjectId.GenerateNewId();
    }
}
