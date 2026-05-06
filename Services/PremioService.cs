using VooApi.Models;
using VooApi.Data;

namespace VooApi.Services
{
    public class PremioService
    {
        private readonly PremioRepository _repository;

        public PremioService(PremioRepository repository)
        {
            _repository = repository;
        }

        public async Task<Premio> CrearAsync(Premio premio)
        {
            await _repository.InsertarAsync(premio);
            return premio;
        }

        public async Task<List<Premio>> ObtenerTodosAsync()
        {
            return await _repository.ObtenerTodosAsync();
        }

        public async Task<Premio?> ObtenerPorIdAsync(string id)
        {
            return await _repository.ObtenerPorIdAsync(id);
        }

        public async Task AsignarGanadorAsync(string premioId, string ganadorId)
        {
            var premio = await _repository.ObtenerPorIdAsync(premioId);
            if (premio == null) return;

            await _repository.AsignarGanadorAsync(premioId, ganadorId);
        }
    }
}