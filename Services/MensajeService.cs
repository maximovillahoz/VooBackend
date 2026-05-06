using VooApi.Models;
using VooApi.Data;

namespace VooApi.Services
{
    public class MensajeService
    {
        private readonly MensajeRepository _repository;

        public MensajeService(MensajeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Mensaje> EnviarAsync(Mensaje mensaje)
        {
            await _repository.InsertarAsync(mensaje);
            return mensaje;
        }

        public async Task<List<Mensaje>> ObtenerPorChatAsync(string chatId)
        {
            return await _repository.ObtenerPorChatAsync(chatId);
        }

        public async Task MarcarLeidoAsync(string id)
        {
            await _repository.MarcarLeidoAsync(id);
        }
    }
}