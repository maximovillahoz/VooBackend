using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VooApi.Models
{
    public class Solicitud
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.UtcNow;
        public bool? Aceptado { get; set; }                     // null hasta que se responda
        public string Tipo { get; set; } = string.Empty;        // "chat" o "juego"
        public string EmisorId { get; set; } = string.Empty;    // FK → Usuario
        public string ReceptorId { get; set; } = string.Empty;  // FK → Usuario
        public string Estado { get; set; } = "pendiente";       // "pendiente", "aceptado", "rechazado"
        public string Contenido { get; set; } = string.Empty;   // texto de la verdad o descripción del reto
    }
}