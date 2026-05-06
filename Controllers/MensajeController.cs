using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MensajeController : ControllerBase
    {
        private readonly MensajeService _service;

        public MensajeController(MensajeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Enviar([FromBody] Mensaje mensaje)
        {
            var enviado = await _service.EnviarAsync(mensaje);
            return Ok(enviado);
        }

        [HttpGet("chat/{chatId}")]
        public async Task<IActionResult> ObtenerPorChat(string chatId)
        {
            var lista = await _service.ObtenerPorChatAsync(chatId);
            return Ok(lista);
        }

        [HttpPatch("{id}/leido")]
        public async Task<IActionResult> MarcarLeido(string id)
        {
            await _service.MarcarLeidoAsync(id);
            return Ok(new { mensaje = "Mensaje marcado como leído" });
        }
    }
}