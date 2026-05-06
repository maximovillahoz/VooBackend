using Microsoft.AspNetCore.SignalR;
using VooApi.Data;
using VooApi.Hubs;
using VooApi.Models;

namespace VooApi.Services
{
    public class SalaService
    {
        private readonly SalaRepository _repository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly ChatRepository _chatRepository;
        private readonly MensajeRepository _mensajeRepository;
        private readonly IHubContext<SalaHub> _hubContext;

        public SalaService(
            SalaRepository repository,
            UsuarioRepository usuarioRepository,
            ChatRepository chatRepository,
            MensajeRepository mensajeRepository,
            IHubContext<SalaHub> hubContext)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
            _chatRepository = chatRepository;
            _mensajeRepository = mensajeRepository;
            _hubContext = hubContext;
        }

        public async Task<Sala> CrearAsync(Sala sala)
        {
            await _repository.InsertarAsync(sala);
            return sala;
        }

        public async Task<Sala?> ObtenerPorIdAsync(string id)
        {
            return await _repository.ObtenerPorIdAsync(id);
        }

        public async Task<Sala?> ObtenerPorCodigoAsync(string codigo)
        {
            return await _repository.ObtenerPorCodigoAsync(codigo);
        }

        public async Task<Sala?> ObtenerPorHostAsync(string hostId)
        {
            return await _repository.ObtenerPorHostAsync(hostId);
        }

        public async Task CerrarSalaAsync(string id)
        {
            await _hubContext.Clients.Group(id).SendAsync("SalaCerrada");

            await Task.Delay(300);

            var usuarios = await _usuarioRepository.ObtenerPorSalaAsync(id);

            var usuariosIds = usuarios
                .Where(u => u.Id != null)
                .Select(u => u.Id!)
                .ToList();

            if (usuariosIds.Count > 0)
            {
                var chats = await _chatRepository.ObtenerPorUsuariosAsync(usuariosIds);

                var chatIds = chats
                    .Where(c => c.Id != null)
                    .Select(c => c.Id!)
                    .ToList();

                if (chatIds.Count > 0)
                {
                    await _mensajeRepository.EliminarPorChatsAsync(chatIds);
                }

                await _chatRepository.EliminarPorUsuariosAsync(usuariosIds);
                await _usuarioRepository.EliminarPorSalaAsync(id);
            }

            await _repository.EliminarAsync(id);
        }

        public async Task ActualizarAsync(string id, Sala sala)
        {
            await _repository.ActualizarAsync(id, sala);
        }

        public async Task EliminarAsync(string id)
        {
            await _repository.EliminarAsync(id);
        }
    }
}