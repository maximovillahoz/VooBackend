using System.ComponentModel.DataAnnotations;

namespace VooApi.Models
{
    public class RegistroInvitadoDto
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

        public bool Verificado { get; set; }

        [Required(ErrorMessage = "El código de sala es obligatorio")]
        [MinLength(6, ErrorMessage = "El código de sala debe tener 6 caracteres")]
        [MaxLength(6, ErrorMessage = "El código de sala debe tener 6 caracteres")]
        public string CodigoSala { get; set; } = string.Empty;

        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90")]
        public double Latitud { get; set; }

        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180")]
        public double Longitud { get; set; }

        [Range(0, 100, ErrorMessage = "La precisión del GPS no es válida")]
        public double Accuracy { get; set; }

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