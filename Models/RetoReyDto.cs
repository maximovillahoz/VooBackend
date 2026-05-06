namespace VooApi.Models
{
    public class RetoReyDto
    {
        // ID del invitado que crea el reto (debe tener >=150 pts)
        public string CreadorId { get; set; } = string.Empty;

        // ID de la sala
        public string SalaId { get; set; } = string.Empty;

        // Qué reto lanza. Valores posibles:
        // "robar_trago", "invitar_bailar", "seguir_ig", "foto_invitado"
        public string TipoReto { get; set; } = string.Empty;

        // Solo para "seguir_ig" → el IG del creador
        public string? IgCreador { get; set; }

        // Solo para "foto_invitado" → ID del invitado seleccionado
        public string? InvitadoSeleccionadoId { get; set; }
    }
}