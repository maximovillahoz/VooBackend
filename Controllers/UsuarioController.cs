using Microsoft.AspNetCore.Mvc;
using VooApi.Models;
using VooApi.Services;
using Microsoft.AspNetCore.SignalR;
using VooApi.Hubs;

namespace VooApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;
        private readonly IHubContext<SalaHub> _hubContext;

        public UsuarioController(
            UsuarioService service,
            IHubContext<SalaHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Usuario usuario)
        {
            var creado = await _service.CrearAsync(usuario);
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
            var usuario = await _service.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        [HttpGet("sala/{salaId}")]
        public async Task<IActionResult> ObtenerPorSala(string salaId)
        {
            var lista = await _service.ObtenerPorSalaAsync(salaId);
            return Ok(lista);
        }
        [HttpPatch("{id}/puntos")]
        public async Task<IActionResult> SumarPuntos(string id, [FromBody] SumarPuntosDto dto)
        {
            var resultado = await _service.SumarPuntosAsync(id, dto.Puntos);

            if (resultado == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            if (resultado.PoderDesbloqueado)
            {
                return Ok(new
                {
                    usuario = resultado.Usuario,
                    poderDesbloqueado = true,
                    nuevoPoder = resultado.NuevoPoder,
                    mensaje = $"¡Has desbloqueado el poder {resultado.NuevoPoder}!"
                });
            }

            return Ok(new
            {
                usuario = resultado.Usuario,
                poderDesbloqueado = false,
                nuevoPoder = (string?)null,
                mensaje = $"Puntos añadidos. Total: {resultado.Usuario.Puntos}"
            });
        }

        [HttpPatch("{id}/banear")]
        public async Task<IActionResult> Banear(string id)
        {
            var usuario = await _service.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            await _service.BanearAsync(id);

            await _hubContext.Clients
                .Group($"usuario-{id}")
                .SendAsync("UsuarioBaneado", new
                {
                    usuarioId = id,
                    salaId = usuario.SalaId
                });

            if (!string.IsNullOrEmpty(usuario.SalaId))
            {
                await _hubContext.Clients
                    .Group(usuario.SalaId)
                    .SendAsync("UsuarioEntrado");
            }

            return Ok(new { mensaje = "Usuario baneado correctamente" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(string id, [FromBody] Usuario usuario)
        {
            await _service.ActualizarAsync(id, usuario);
            return Ok(usuario);
        }

        [HttpPatch("{id}/salir")]
        public async Task<IActionResult> SalirDeSala(string id)
        {
            await _service.SalirDeSalaAsync(id);
            return Ok(new { mensaje = "Has salido de la sala" });
        }

    }            
    public class SumarPuntosDto
    {
        public int Puntos { get; set; }
    }                
}