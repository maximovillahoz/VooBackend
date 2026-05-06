using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VooApi.Models
{
    public class Poder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string NivelId { get; set; } = string.Empty;     // "chismoso", "cupido", "rey"
        public int PuntosNecesarios { get; set; }               // 50, 100, 200
        public string ConceptoPoder { get; set; } = string.Empty; // descripción de qué desbloquea
    }
}