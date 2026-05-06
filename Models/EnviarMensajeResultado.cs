namespace VooApi.Models
{
    public class EnviarMensajeResultado
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public Mensaje? MensajeEnviado { get; set; }
    }
}