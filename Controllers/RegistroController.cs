using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistroController : ControllerBase
    {
        private readonly RegistroService _service;

        public RegistroController(RegistroService service)
        {
            _service = service;
        }

        // POST /registro/host
        // Flutter llama a esto cuando el host termina de configurar la sala
        [HttpPost("host")]
        public async Task<IActionResult> RegistrarHost([FromBody] RegistroHostDto dto)
        {
            var resultado = await _service.RegistrarHostAsync(dto);

            if (!resultado.Exito)
                return BadRequest(new { mensaje = resultado.Mensaje });

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                usuario = resultado.Usuario,
                sala = resultado.Sala,
                codigoSala = resultado.Sala.CodigoSala
            });
        }

        // POST /registro/invitado
        // Flutter llama a esto cuando el invitado termina el registro
        [HttpPost("invitado")]
        public async Task<IActionResult> RegistrarInvitado([FromBody] RegistroInvitadoDto dto)
        {
            var resultado = await _service.RegistrarInvitadoAsync(dto);

            if (!resultado.Exito)
                return BadRequest(new { mensaje = resultado.Mensaje });

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                usuario = resultado.Usuario,
                sala = resultado.Sala
            });
        }
    }
}