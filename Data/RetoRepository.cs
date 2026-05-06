using MongoDB.Driver;
using VooApi.Database;
using VooApi.Models;

namespace VooApi.Data
{
    public class RetoRepository
    {
        private readonly IMongoCollection<Reto> _collection;

        public RetoRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Reto>("retos");
        }

        public async Task InsertarAsync(Reto reto)
        {
            await _collection.InsertOneAsync(reto);
        }

        public async Task<Reto?> ObtenerPorIdAsync(string id)
        {
            return await _collection.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Reto>> ObtenerTodosAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        // Obtener solo los retos activos
        public async Task<List<Reto>> ObtenerActivosAsync()
        {
            return await _collection.Find(r => r.EstadoReto == "activo").ToListAsync();
        }

        // Cambiar el estado de un reto
        public async Task ActualizarEstadoAsync(string id, string nuevoEstado)
        {
            var update = Builders<Reto>.Update.Set(r => r.EstadoReto, nuevoEstado);
            await _collection.UpdateOneAsync(r => r.Id == id, update);
        }

        public async Task ActualizarAsync(string id, Reto reto)
        {
            await _collection.ReplaceOneAsync(r => r.Id == id, reto);
        }

        public async Task EliminarAsync(string id)
        {
            await _collection.DeleteOneAsync(r => r.Id == id);
        }

        public async Task ActualizarCamposAsync(
            string id,
            string estado,
            DateTime? horaActivacion,
            TimeSpan? duracion)
        {
            var update = Builders<Reto>.Update
                .Set(r => r.EstadoReto, estado)
                .Set(r => r.HoraActivacion, horaActivacion)
                .Set(r => r.Duracion, duracion);

            await _collection.UpdateOneAsync(r => r.Id == id, update);
        }
    }
}