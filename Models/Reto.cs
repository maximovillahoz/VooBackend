using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VooApi.Models
{
    public class Reto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Tipo { get; set; } = string.Empty;        // "generado", "por host", "por invitado"
        public string Concepto { get; set; } = string.Empty;    // descripción del reto
        public int Puntos { get; set; }                         // puntos que da al completarlo
        public string? PremioId { get; set; }                   // FK → Premio (opcional)
        public DateTime? HoraActivacion { get; set; }
        public TimeSpan? Duracion { get; set; }
        public string EstadoReto { get; set; } = "proximo";     // "proximo", "activo", "completado"
    }
}