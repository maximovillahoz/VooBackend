using MongoDB.Driver;
using VooApi.Database;
using VooApi.Models;

namespace VooApi.Data
{
    public class PremioRepository
    {
        private readonly IMongoCollection<Premio> _collection;

        public PremioRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Premio>("premios");
        }

        public async Task InsertarAsync(Premio premio)
        {
            await _collection.InsertOneAsync(premio);
        }

        public async Task<Premio?> ObtenerPorIdAsync(string id)
        {
            return await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Premio>> ObtenerTodosAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        // Asignar ganador a un premio
        public async Task AsignarGanadorAsync(string id, string ganadorId)
        {
            var update = Builders<Premio>.Update.Set(p => p.GanadorId, ganadorId);
            await _collection.UpdateOneAsync(p => p.Id == id, update);
        }

        public async Task EliminarAsync(string id)
        {
            await _collection.DeleteOneAsync(p => p.Id == id);
        }
    }
}