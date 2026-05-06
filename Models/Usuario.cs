using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VooApi.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Tipo { get; set; } = string.Empty;        // "host" o "invited"
        public string Nombre { get; set; } = string.Empty;
        public bool Sexo { get; set; } = false;
        public DateTime FechaNacimiento { get; set; }
        public string? Ig { get; set; }                         // opcional
        public string Foto { get; set; } = string.Empty;       
         // URL de la imagen
        public bool Verificado { get; set; } = false;
        public bool AceptaTerminos { get; set; } = false;
        public bool AceptaPrivacidad { get; set; } = false;
        public bool AceptaBiometria { get; set; } = false;
        public DateTime? FechaAceptacionTerminos { get; set; }
        public bool DentroRadio { get; set; } = true;
        public DateTime? UltimaVerificacion { get; set; }
        public string Estado { get; set; } = string.Empty;      // "verde", "amarillo", "rojo"
        public List<string> Respuestas { get; set; } = new();   // las 3 respuestas del login
        public string? SalaId { get; set; }                     // FK → Sala
        public int Puntos { get; set; } = 0;
        public int Match { get; set; } = 0;
        public string? NivelId { get; set; }                    // FK → Poder
        public List<string> RetosCumplidos { get; set; } = new(); // FK → Reto
        public List<string> Premios { get; set; } = new();      // FK → Premio
        public bool Baneado { get; set; } = false;
        public string? DeviceId { get; set; }
    }
}