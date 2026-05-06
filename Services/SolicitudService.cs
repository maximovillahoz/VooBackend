using VooApi.Models;
using VooApi.Data;

namespace VooApi.Services
{
    public class SolicitudService
    {
        private readonly SolicitudRepository _repository;

        public SolicitudService(SolicitudRepository repository)
        {
            _repository = repository;
        }

        public async Task<Solicitud> EnviarAsync(Solicitud solicitud)
        {
            await _repository.InsertarAsync(solicitud);
            return solicitud;
        }

        public async Task<List<Solicitud>> ObtenerRecibidosAsync(string receptorId)
        {
            return await _repository.ObtenerPorReceptorAsync(receptorId);
        }

        public async Task<List<Solicitud>> ObtenerEnviadosAsync(string emisorId)
        {
            return await _repository.ObtenerPorEmisorAsync(emisorId);
        }

        public async Task AceptarAsync(string id)
        {
            await _repository.ActualizarEstadoAsync(id, "aceptado", true);
        }

        public async Task RechazarAsync(string id)
        {
            await _repository.ActualizarEstadoAsync(id, "rechazado", false);
        }
    }
}