namespace VooApi.Models
{
    public class AceptarSolicitudResultado
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int PuntosGanados { get; set; }
        public string EmisorId { get; set; } = string.Empty;
        public string ReceptorId { get; set; } = string.Empty;

        // El chat creado automáticamente
        public string? ChatId { get; set; }

        // El primer mensaje (la verdad o el reto)
        public Mensaje? PrimerMensaje { get; set; }
    }
}