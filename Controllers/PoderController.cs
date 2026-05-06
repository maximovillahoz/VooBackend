using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PoderController : ControllerBase
    {
        private readonly PoderService _service;

        public PoderController(PoderService service)
        {
            _service = service;
        }

        // GET /poder
        // Devuelve todos los poderes del sistema
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var lista = await _service.ObtenerTodosAsync();
            return Ok(lista);
        }

        // GET /poder/usuario/{usuarioId}
        // Devuelve los poderes disponibles para un usuario concreto
        // Flutter usa esto para saber qué botones mostrar
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerDisponibles(string usuarioId)
        {
            var poderes = await _service.ObtenerPoderesDisponiblesAsync(usuarioId);
            return Ok(poderes);
        }

        // GET /poder/permiso/{usuarioId}/{nivelId}
        // Comprueba si un usuario puede usar un poder concreto
        // Flutter llama a esto antes de ejecutar una acción de poder
        [HttpGet("permiso/{usuarioId}/{nivelId}")]
        public async Task<IActionResult> TienePermiso(string usuarioId, string nivelId)
        {
            var tienePermiso = await _service.TienePermisoAsync(usuarioId, nivelId);

            if (!tienePermiso)
            {
                return Ok(new
                {
                    permitido = false,
                    mensaje = "No tienes suficientes puntos para usar este poder"
                });
            }

            return Ok(new
            {
                permitido = true,
                mensaje = "Puedes usar este poder"
            });
        }

        // GET /poder/nivel/{nivelId}
        // Devuelve la info completa de un poder
        // Flutter usa esto para mostrar la descripción en pantalla
        [HttpGet("nivel/{nivelId}")]
        public async Task<IActionResult> ObtenerPorNivel(string nivelId)
        {
            var poder = await _service.ObtenerPorNivelAsync(nivelId);
            if (poder == null) return NotFound(new { mensaje = "Poder no encontrado" });
            return Ok(poder);
        }

        // POST /poder
        // Crear un poder en MongoDB
        // Solo para inicializar los 3 poderes del sistema
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Poder poder)
        {
            var creado = await _service.CrearAsync(poder);
            return Ok(creado);
        }
    }
}