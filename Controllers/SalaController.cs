using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalaController : ControllerBase
    {
        private readonly SalaService _service;

        public SalaController(SalaService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Sala sala)
        {
            var creada = await _service.CrearAsync(sala);
            return Ok(creada);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(string id)
        {
            var sala = await _service.ObtenerPorIdAsync(id);
            if (sala == null) return NotFound();
            return Ok(sala);
        }

        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> ObtenerPorCodigo(string codigo)
        {
            var sala = await _service.ObtenerPorCodigoAsync(codigo);
            if (sala == null) return NotFound();
            return Ok(sala);
        }

        [HttpGet("host/{hostId}")]
        public async Task<IActionResult> ObtenerPorHost(string hostId)
        {
            var sala = await _service.ObtenerPorHostAsync(hostId);
            if (sala == null) return NotFound();
            return Ok(sala);
        }

        [HttpPatch("{id}/cerrar")]
        public async Task<IActionResult> CerrarSala(string id)
        {
            await _service.CerrarSalaAsync(id);

            return Ok(new { mensaje = "Sala cerrada y datos eliminados correctamente" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(string id, [FromBody] Sala sala)
        {
            await _service.ActualizarAsync(id, sala);
            return Ok(sala);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(string id)
        {
            await _service.EliminarAsync(id);
            return Ok(new { mensaje = "Sala eliminada correctamente" });
        }
    }
}