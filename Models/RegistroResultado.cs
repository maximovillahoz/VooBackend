namespace VooApi.Models
{
    public class RegistroResultado
    {
        public Usuario Usuario { get; set; } = null!;
        public Sala Sala { get; set; } = null!;
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}