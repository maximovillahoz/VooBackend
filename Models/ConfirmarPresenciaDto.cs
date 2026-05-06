using System.ComponentModel.DataAnnotations;

namespace VooApi.Models
{
    public class ConfirmarPresenciaDto
    {
        [Required(ErrorMessage = "El UsuarioId es obligatorio")]
        public string UsuarioId { get; set; } = string.Empty;

        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90")]
        public double Latitud { get; set; }

        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180")]
        public double Longitud { get; set; }

        [Range(0, 100, ErrorMessage = "La precisión del GPS no es válida")]
        public double Accuracy { get; set; }
    }
}