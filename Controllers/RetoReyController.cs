using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RetoReyController : ControllerBase
    {
        private readonly RetoReyService _service;

        public RetoReyController(RetoReyService service)
        {
            _service = service;
        }

        // POST /retorey/crear
        // Flutter llama a esto cuando el Rey lanza un reto a la sala
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] RetoReyDto dto)
        {
            var resultado = await _service.CrearRetoAsync(dto);
            if (!resultado.Exito)
                return BadRequest(new { mensaje = resultado.Mensaje });

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                retoId = resultado.RetoId
            });
        }

        // POST /retorey/verificar
        // Flutter llama a esto cuando el Rey escanea el QR del cumplidor
        [HttpPost("verificar")]
        public async Task<IActionResult> Verificar([FromBody] VerificacionReyDto dto)
        {
            var resultado = await _service.VerificarRetoAsync(dto);
            if (!resultado.Exito)
                return BadRequest(new { mensaje = resultado.Mensaje });

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                puntosGanados = resultado.PuntosGanados
            });
        }
    }
}