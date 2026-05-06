using System.ComponentModel.DataAnnotations;

namespace VooApi.Models
{
    public class EnviarMensajeDto
    {
        [Required(ErrorMessage = "El ChatId es obligatorio")]
        public string ChatId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El EmisorId es obligatorio")]
        public string EmisorId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ReceptorId es obligatorio")]
        public string ReceptorId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El contenido es obligatorio")]
        [MinLength(1, ErrorMessage = "El mensaje no puede estar vacío")]
        [MaxLength(500, ErrorMessage = "El mensaje no puede tener más de 500 caracteres")]
        public string Contenido { get; set; } = string.Empty;
    }
}