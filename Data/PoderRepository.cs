using MongoDB.Driver;
using VooApi.Database;
using VooApi.Models;

namespace VooApi.Data
{
    public class PoderRepository
    {
        private readonly IMongoCollection<Poder> _collection;

        public PoderRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Poder>("poderes");
        }

        public async Task InsertarAsync(Poder poder)
        {
            await _collection.InsertOneAsync(poder);
        }

        public async Task<Poder?> ObtenerPorIdAsync(string id)
        {
            return await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Poder>> ObtenerTodosAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        // Obtener el poder que corresponde a unos puntos dados
        public async Task<Poder?> ObtenerPorPuntosAsync(int puntos)
        {
            return await _collection
                .Find(p => p.PuntosNecesarios <= puntos)
                .SortByDescending(p => p.PuntosNecesarios)
                .FirstOrDefaultAsync();
        }
    }
}