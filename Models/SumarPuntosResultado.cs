namespace VooApi.Models
{
    public class SumarPuntosResultado
    {
        // El usuario completo con sus puntos y poder actualizados
        public required VooApi.Models.Usuario Usuario { get; set; }

        // ¿Acaba de desbloquear un poder nuevo en esta acción?
        public bool PoderDesbloqueado { get; set; }

        // Qué poder ha desbloqueado (si es que ha desbloqueado alguno)
        // Será null si no ha desbloqueado nada nuevo
        public string? NuevoPoder { get; set; }
    }
}