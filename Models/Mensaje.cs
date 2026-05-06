using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VooApi.Models
{
    public class Mensaje
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string ChatId { get; set; } = string.Empty;      // FK → Chat
        public string Tipo { get; set; } = string.Empty;        // "texto", "verdad", "reto"
        public string EmisorId { get; set; } = string.Empty;    // FK → Usuario
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;
        public bool Leido { get; set; } = false;
    }
}