using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VooApi.Models
{
    public class RetoCompletado
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string RetoId { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public string UsuarioEscaneadoId { get; set; } = string.Empty;

        public DateTime FechaCompletado { get; set; } = DateTime.UtcNow;
    }
}