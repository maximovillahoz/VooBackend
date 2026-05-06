using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;
using Microsoft.AspNetCore.SignalR;
using VooApi.Hubs;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VerdadRetoController : ControllerBase
    {
        private readonly VerdadRetoService _service;
        private readonly IHubContext<SalaHub> _hubContext;

        public VerdadRetoController(
            VerdadRetoService service,
            IHubContext<SalaHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        // GET /verdadreto/obtener
        [HttpGet("obtener")]
        public IActionResult ObtenerVerdadYReto()
        {
            var resultado = _service.ObtenerVerdadYReto();
            return Ok(resultado);
        }

        // POST /verdadreto/aceptar/{solicitudId}
        // Acepta la solicitud, crea el chat y manda el primer mensaje
        [HttpPost("aceptar/{solicitudId}")]
        public async Task<IActionResult> Aceptar(string solicitudId)
        {
            var resultado = await _service.AceptarSolicitudAsync(solicitudId);

            if (!resultado.Exito)
                return BadRequest(new { mensaje = resultado.Mensaje });

            await _hubContext.Clients
                .Group($"usuario-{resultado.EmisorId}")
                .SendAsync("SolicitudAceptada", new
                {
                    solicitudId,
                    chatId = resultado.ChatId,
                    primerMensaje = resultado.PrimerMensaje,
                    receptorId = resultado.ReceptorId
                });

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                puntosGanados = resultado.PuntosGanados,
                chatId = resultado.ChatId,
                primerMensaje = resultado.PrimerMensaje
            });
        }

        // POST /verdadreto/rechazar/{solicitudId}
        // Rechaza la solicitud
        [HttpPost("rechazar/{solicitudId}")]
        public async Task<IActionResult> Rechazar(string solicitudId)
        {
            var resultado = await _service.RechazarSolicitudAsync(solicitudId);

            if (!resultado.Exito)
                return BadRequest(new { mensaje = resultado.Mensaje });

            await _hubContext.Clients
                .Group($"usuario-{resultado.EmisorId}")
                .SendAsync("SolicitudRechazada", new
                {
                    solicitudId,
                    receptorId = resultado.ReceptorId,
                    mensaje = resultado.Mensaje
                });

            return Ok(new { mensaje = resultado.Mensaje });
        }
    }
}