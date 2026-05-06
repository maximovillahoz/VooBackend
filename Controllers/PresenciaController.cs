using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PresenciaController : ControllerBase
    {
        private readonly PresenciaService _service;

        public PresenciaController(PresenciaService service)
        {
            _service = service;
        }

        // POST /presencia/confirmar
        // Flutter llama a esto cuando el usuario confirma su ubicación
        [HttpPost("confirmar")]
        public async Task<IActionResult> Confirmar([FromBody] ConfirmarPresenciaDto dto)
        {
            var resultado = await _service.ConfirmarPresenciaAsync(dto);

            if (!resultado.Exito)
                return BadRequest(new
                {
                    mensaje = resultado.Mensaje,
                    dentroRadio = resultado.DentroRadio
                });

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                dentroRadio = resultado.DentroRadio
            });
        }
    }
}