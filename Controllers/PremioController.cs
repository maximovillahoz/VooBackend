using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PremioController : ControllerBase
    {
        private readonly PremioService _service;

        public PremioController(PremioService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Premio premio)
        {
            var creado = await _service.CrearAsync(premio);
            return Ok(creado);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var lista = await _service.ObtenerTodosAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(string id)
        {
            var premio = await _service.ObtenerPorIdAsync(id);
            if (premio == null) return NotFound();
            return Ok(premio);
        }

        [HttpPatch("{premioId}/ganador/{ganadorId}")]
        public async Task<IActionResult> AsignarGanador(string premioId, string ganadorId)
        {
            await _service.AsignarGanadorAsync(premioId, ganadorId);
            return Ok(new { mensaje = "Ganador asignado correctamente" });
        }
    }
}