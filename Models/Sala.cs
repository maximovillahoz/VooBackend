using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace VooApi.Models
{
    public class Sala
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string HostId { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Contexto { get; set; } = string.Empty;
        public int Aforo { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public int CodigoPostal { get; set; }
        public List<string> Premios { get; set; } = new();
        public int Invitados { get; set; } = 0;
        public int Baneados { get; set; } = 0;
        public string? Incidencias { get; set; }
        public DateTime FechaHoraInicio { get; set; } = DateTime.UtcNow;
        public DateTime? FechaHoraFin { get; set; }

        // Campos nuevos
        public string CodigoSala { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}