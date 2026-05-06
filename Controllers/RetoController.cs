using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RetoController : ControllerBase
    {
        private readonly RetoService _service;

        public RetoController(RetoService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Reto reto)
        {
            var creado = await _service.CrearAsync(reto);
            return Ok(creado);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var lista = await _service.ObtenerTodosAsync();
            return Ok(lista);
        }

        [HttpGet("activos")]
        public async Task<IActionResult> ObtenerActivos()
        {
            var lista = await _service.ObtenerActivosAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(string id)
        {
            var reto = await _service.ObtenerPorIdAsync(id);
            if (reto == null) return NotFound();
            return Ok(reto);
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activar(string id)
        {
            await _service.ActivarRetoAsync(id);
            return Ok(new { mensaje = "Reto activado" });
        }

        [HttpPatch("{id}/completar")]
        public async Task<IActionResult> Completar(string id)
        {
            await _service.CompletarRetoAsync(id);
            return Ok(new { mensaje = "Reto completado" });
        }

        [HttpGet("timeline/{usuarioId}")]
        public async Task<IActionResult> ObtenerTimeline(string usuarioId)
        {
            var resultado = await _service.ObtenerTimelineAsync(usuarioId);
            return Ok(resultado);
        }

        [HttpPost("completar-actual")]
        public async Task<IActionResult> CompletarActual([FromBody] CompletarRetoActualDto dto)
        {
            var resultado = await _service.CompletarRetoActivoAsync(
                dto.UsuarioId,
                dto.UsuarioEscaneadoId
            );

            return Ok(resultado);
        }
    }

    public class CompletarRetoActualDto
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string UsuarioEscaneadoId { get; set; } = string.Empty;
    }
}