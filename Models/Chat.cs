using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VooApi.Models
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string EmisorId { get; set; } = string.Empty;    // FK → Usuario
        public string ReceptorId { get; set; } = string.Empty;  // FK → Usuario
        public bool Activo { get; set; } = true;
    }
}