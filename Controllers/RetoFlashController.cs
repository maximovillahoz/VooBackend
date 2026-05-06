using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RetoFlashController : ControllerBase
    {
        private readonly RetoFlashService _service;

        public RetoFlashController(RetoFlashService service)
        {
            _service = service;
        }

        // POST /retoflash/verificar
        // Flutter llama a esto cuando el usuario completa un reto flash
        [HttpPost("verificar")]
        public async Task<IActionResult> Verificar([FromBody] ProgresoReto progreso)
        {
            var resultado = await _service.VerificarRetoAsync(progreso);

            if (!resultado.Exito)
                return BadRequest(new { mensaje = resultado.Mensaje });

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                puntosGanados = 30
            });
        }
    }
}