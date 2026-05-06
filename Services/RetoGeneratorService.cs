using VooApi.Models;
using VooApi.Data;

namespace VooApi.Services
{
    public class RetoGeneratorService
    {
        private readonly RetoRepository _repository;
        private readonly Random _random = new();

        private readonly List<string> _tipos = new()
        {
            "cualquier_escaneo",
            "misma_edad_mujer",
            "nombre_a",
            "verde_misma_edad",
            "amarillo_2h_1m",
            "match_0pts",
            "misma_edad_mismo_estado",
            "mismo_estado",
            "mas_joven",
            "misma_edad_diferente_estado"
        };

        public RetoGeneratorService(RetoRepository repository)
        {
            _repository = repository;
        }

        public async Task GenerarRetoAsync()
        {
            var tipo = _tipos[_random.Next(_tipos.Count)];

            var reto = new Reto
            {
                Tipo = tipo,
                Concepto = GenerarConcepto(tipo),
                Puntos = _random.Next(10, 50),
                HoraActivacion = DateTime.UtcNow,
                Duracion = TimeSpan.FromMinutes(30),
                EstadoReto = "activo"
            };

            var existentes = await _repository.ObtenerTodosAsync();
            if (existentes.Count > 0) return;

            await _repository.InsertarAsync(reto);
        }

        private string GenerarConcepto(string tipo)
        {
            return tipo switch
            {
                "cualquier_escaneo" => "Escanea a cualquier persona",
                "mas_joven" => "Escanea a alguien más joven que tú",
                "mismo_estado" => "Escanea a alguien con tu mismo estado",
                "nombre_a" => "Escanea a alguien cuyo nombre tenga A",
                _ => "Completa el reto especial"
            };
        }
    }
}