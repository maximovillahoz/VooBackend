using System.ComponentModel.DataAnnotations;

namespace VooApi.Models
{
    public class SumarPuntosDto
    {
        [Range(1, 200, ErrorMessage = "Los puntos deben estar entre 1 y 200")]
        public int Puntos { get; set; }
    }
}