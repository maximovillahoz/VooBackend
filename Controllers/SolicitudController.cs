using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SolicitudController : ControllerBase
    {
        private readonly SolicitudService _service;

        public SolicitudController(SolicitudService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Enviar([FromBody] Solicitud solicitud)
        {
            var creada = await _service.EnviarAsync(solicitud);
            return Ok(creada);
        }

        [HttpGet("recibidas/{receptorId}")]
        public async Task<IActionResult> ObtenerRecibidas(string receptorId)
        {
            var lista = await _service.ObtenerRecibidosAsync(receptorId);
            return Ok(lista);
        }

        [HttpGet("enviadas/{emisorId}")]
        public async Task<IActionResult> ObtenerEnviadas(string emisorId)
        {
            var lista = await _service.ObtenerEnviadosAsync(emisorId);
            return Ok(lista);
        }

        [HttpPatch("{id}/aceptar")]
        public async Task<IActionResult> Aceptar(string id)
        {
            await _service.AceptarAsync(id);
            return Ok(new { mensaje = "Solicitud aceptada" });
        }

        [HttpPatch("{id}/rechazar")]
        public async Task<IActionResult> Rechazar(string id)
        {
            await _service.RechazarAsync(id);
            return Ok(new { mensaje = "Solicitud rechazada" });
        }
    }
}