using System.ComponentModel.DataAnnotations;

namespace VooApi.Models
{
    public class RegistroHostDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
        [MaxLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "La foto es obligatoria")]
        [MinLength(10, ErrorMessage = "La foto no es válida")]
        public string Foto { get; set; } = string.Empty;

        public string? Ig { get; set; }

        public bool Sexo { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [RegularExpression("^(soltero|amigos|pareja)$",
            ErrorMessage = "El estado debe ser soltero, amigos o pareja")]
        public string Estado { get; set; } = string.Empty;
        [Required(ErrorMessage = "Las respuestas son obligatorias")]
        [MinLength(3, ErrorMessage = "Debes responder exactamente 3 preguntas")]
        [MaxLength(3, ErrorMessage = "Debes responder exactamente 3 preguntas")]
        public List<string> Respuestas { get; set; } = new();

        [Required(ErrorMessage = "El nombre de la sala es obligatorio")]
        [MinLength(2, ErrorMessage = "El nombre de la sala debe tener al menos 2 caracteres")]
        [MaxLength(100, ErrorMessage = "El nombre de la sala no puede tener más de 100 caracteres")]
        public string NombreSala { get; set; } = string.Empty;

        [Required(ErrorMessage = "El contexto es obligatorio")]
        [MinLength(2, ErrorMessage = "El contexto no es válido")]
        public string Contexto { get; set; } = string.Empty;

        [Range(1, 30, ErrorMessage = "El aforo máximo en versión demo es 30 personas")]
        public int Aforo { get; set; } = 30;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [MaxLength(200, ErrorMessage = "La dirección no puede tener más de 200 caracteres")]
        public string Direccion { get; set; } = string.Empty;

        [Range(1000, 99999, ErrorMessage = "El código postal no es válido")]
        public int CodigoPostal { get; set; }

        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90")]
        public double LatitudSala { get; set; }

        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180")]
        public double LongitudSala { get; set; }

        [Required(ErrorMessage = "El premio mayor es obligatorio")]
        [MaxLength(200, ErrorMessage = "El premio mayor no puede tener más de 200 caracteres")]
        public string PremioMayor { get; set; } = string.Empty;

        public List<string> PremiosFlash { get; set; } = new();

        [Range(typeof(bool), "true", "true",
    ErrorMessage = "Debes aceptar los términos y condiciones")]
        public bool AceptaTerminos { get; set; }

        [Range(typeof(bool), "true", "true",
            ErrorMessage = "Debes aceptar la política de privacidad")]
        public bool AceptaPrivacidad { get; set; }

        [Range(typeof(bool), "true", "true",
            ErrorMessage = "Debes aceptar el tratamiento de datos biométricos")]
        public bool AceptaBiometria { get; set; }

        public string? DeviceId { get; set; }
    }
}