namespace VooApi.Models
{
    public class ResultadoPresencia
    {
        public bool Exito { get; set; }
        public bool DentroRadio { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}