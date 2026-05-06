namespace VooApi.Models
{
    public class ProgresoReto
    {
        // ID del usuario que está haciendo el reto
        public string UsuarioId { get; set; } = string.Empty;

        // ID de la sala donde se hace el reto
        public string SalaId { get; set; } = string.Empty;

        // Qué reto es. Valores posibles:
        // "misma_edad_mujer", "nombre_a", "verde_misma_edad",
        // "amarillo_2h_1m", "match_0pts", "misma_edad_mismo_estado",
        // "mismo_estado", "mas_joven", "misma_edad_diferente_estado",
        // "cualquier_escaneo"
        public string TipoReto { get; set; } = string.Empty;

        // IDs de todos los usuarios escaneados para completar el reto
        // Para retos de 1 escaneo → lista con 1 elemento
        // Para retos de 2 escaneos → lista con 2 elementos
        // Para retos de 3 escaneos → lista con 3 elementos
        public List<string> EscaneadosIds { get; set; } = new();
    }
}