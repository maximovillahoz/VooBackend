using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VooApi.Models
{
    public class Premio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;        // "sorpresa" o "creado"
        public string? Motivo { get; set; }                     // "ranking" o "reto"
        public string? GanadorId { get; set; }                  // FK → Usuario
    }
}