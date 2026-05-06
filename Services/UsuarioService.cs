using VooApi.Models;
using VooApi.Data;
using Microsoft.AspNetCore.SignalR;
using VooApi.Hubs;

namespace VooApi.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _repository;
        private readonly ChatRepository _chatRepository;
        private readonly MensajeRepository _mensajeRepository;
        private readonly SolicitudRepository _solicitudRepository;
        private readonly RetoCompletadoRepository _retoCompletadoRepository;
        private readonly IHubContext<SalaHub> _hubContext;

        public UsuarioService(
            UsuarioRepository repository,
            ChatRepository chatRepository,
            MensajeRepository mensajeRepository,
            SolicitudRepository solicitudRepository,
            RetoCompletadoRepository retoCompletadoRepository,
            IHubContext<SalaHub> hubContext)
        {
            _repository = repository;
            _chatRepository = chatRepository;
            _mensajeRepository = mensajeRepository;
            _solicitudRepository = solicitudRepository;
            _retoCompletadoRepository = retoCompletadoRepository;
            _hubContext = hubContext;
        }

        public async Task<SumarPuntosResultado?> SumarPuntosAsync(string id, int puntosASumar)
        {
            var usuario = await _repository.ObtenerPorIdAsync(id);
            if (usuario == null) return null;

            var poderAnterior = usuario.NivelId ?? "ninguno";

            usuario.Puntos += puntosASumar;

            var poderNuevo = usuario.Puntos switch
            {
                >= 200 => "rey",
                >= 100 => "cupido",
                >= 50  => "chismoso",
                _      => "ninguno"
            };

            usuario.NivelId = poderNuevo;

            var esPoderNuevo = poderAnterior != poderNuevo;

            await _repository.ActualizarPuntosAsync(id, usuario.Puntos, usuario.NivelId);

            return new SumarPuntosResultado
            {
                Usuario = usuario,
                PoderDesbloqueado = esPoderNuevo,
                NuevoPoder = esPoderNuevo ? poderNuevo : null
            };
        }

        public async Task<Usuario> CrearAsync(Usuario usuario)
        {
            await _repository.InsertarAsync(usuario);
            return usuario;
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _repository.ObtenerTodosAsync();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(string id)
        {
            return await _repository.ObtenerPorIdAsync(id);
        }

        public async Task<List<Usuario>> ObtenerPorSalaAsync(string salaId)
        {
            return await _repository.ObtenerPorSalaAsync(salaId);
        }

        public async Task BanearAsync(string id)
        {
            var usuario = await _repository.ObtenerPorIdAsync(id);

            if (usuario == null) return;

            usuario.Baneado = true;

            await _repository.ActualizarAsync(id, usuario);

            // 🔥 FORZAR SALIDA
            await _hubContext.Clients.Group(id).SendAsync("UsuarioBaneado", new
            {
                usuarioId = id
            });
        }

        public async Task ActualizarAsync(string id, Usuario usuario)
        {
            await _repository.ActualizarAsync(id, usuario);
        }

        public async Task SalirDeSalaAsync(string id)
        {
            var usuario = await _repository.ObtenerPorIdAsync(id);
            if (usuario == null) return;

            var salaId = usuario.SalaId;

            if (usuario.Tipo == "invited")
            {
                var chats = await _chatRepository.ObtenerPorUsuarioAsync(id);

                var chatIds = chats
                    .Where(c => c.Id != null)
                    .Select(c => c.Id!)
                    .ToList();

                if (chatIds.Count > 0)
                {
                    await _mensajeRepository.EliminarPorChatsAsync(chatIds);
                }

                await _chatRepository.EliminarPorUsuarioAsync(id);
                await _solicitudRepository.EliminarPorUsuarioAsync(id);
                await _retoCompletadoRepository.EliminarPorUsuarioAsync(id);
                await _repository.EliminarAsync(id);
            }
            else
            {
                await _repository.SalirDeSalaAsync(id);
            }

            if (!string.IsNullOrEmpty(salaId))
            {
                await _hubContext.Clients.Group(salaId).SendAsync("UsuarioSalio", new
                {
                    usuarioId = id
                });
            }
        }
    }
}