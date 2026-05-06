namespace VooApi.Models
{
    // Lo que Flutter muestra en la lista de chats
    public class ChatResumen
    {
        public string ChatId { get; set; } = string.Empty;
        public string OtroUsuarioId { get; set; } = string.Empty;
        public string OtroUsuarioNombre { get; set; } = string.Empty;
        public string OtroUsuarioFoto { get; set; } = string.Empty;
        public string OtroUsuarioEstado { get; set; } = string.Empty;
        public string UltimoMensaje { get; set; } = string.Empty;
        public DateTime? UltimoMensajeFecha { get; set; }
        public int MensajesNoLeidos { get; set; }
        public bool Activo { get; set; }
    }
}