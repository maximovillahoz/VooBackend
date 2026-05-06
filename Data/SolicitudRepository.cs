using MongoDB.Driver;
using VooApi.Database;
using VooApi.Models;

namespace VooApi.Data
{
    public class SolicitudRepository
    {
        private readonly IMongoCollection<Solicitud> _collection;

        public SolicitudRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Solicitud>("solicitudes");
        }

        public async Task InsertarAsync(Solicitud solicitud)
        {
            await _collection.InsertOneAsync(solicitud);
        }

        public async Task<Solicitud?> ObtenerPorIdAsync(string id)
        {
            return await _collection.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        // Obtener todas las solicitudes recibidas por un usuario
        public async Task<List<Solicitud>> ObtenerPorReceptorAsync(string receptorId)
        {
            return await _collection.Find(s => s.ReceptorId == receptorId).ToListAsync();
        }

        // Obtener todas las solicitudes enviadas por un usuario
        public async Task<List<Solicitud>> ObtenerPorEmisorAsync(string emisorId)
        {
            return await _collection.Find(s => s.EmisorId == emisorId).ToListAsync();
        }

        // Actualizar el estado de la solicitud (aceptado/rechazado)
        public async Task ActualizarEstadoAsync(string id, string estado, bool aceptado)
        {
            var update = Builders<Solicitud>.Update
                .Set(s => s.Estado, estado)
                .Set(s => s.Aceptado, aceptado);
            await _collection.UpdateOneAsync(s => s.Id == id, update);
        }

        public async Task EliminarAsync(string id)
        {
            await _collection.DeleteOneAsync(s => s.Id == id);
        }
        // Añadir al final de SolicitudRepository.cs
        public async Task<Solicitud?> ObtenerPorParticipantesYEstadoAsync(
            string emisorId, string receptorId, string estado)
        {
            return await _collection.Find(s =>
                ((s.EmisorId == emisorId && s.ReceptorId == receptorId) ||
                (s.EmisorId == receptorId && s.ReceptorId == emisorId)) &&
                s.Estado == estado
            ).FirstOrDefaultAsync();
        }

        public async Task EliminarPorUsuarioAsync(string usuarioId)
        {
            await _collection.DeleteManyAsync(s =>
                s.EmisorId == usuarioId || s.ReceptorId == usuarioId
            );
        }
    }
}