using MongoDB.Driver;
using VooApi.Database;
using VooApi.Models;

namespace VooApi.Data
{
    public class MensajeRepository
    {
        private readonly IMongoCollection<Mensaje> _collection;

        public MensajeRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Mensaje>("mensajes");
        }

        public async Task InsertarAsync(Mensaje mensaje)
        {
            await _collection.InsertOneAsync(mensaje);
        }

        public async Task<Mensaje?> ObtenerPorIdAsync(string id)
        {
            return await _collection.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        // Obtener todos los mensajes de un chat
        public async Task<List<Mensaje>> ObtenerPorChatAsync(string chatId)
        {
            return await _collection
                .Find(m => m.ChatId == chatId)
                .SortBy(m => m.FechaHora)
                .ToListAsync();
        }

        // Marcar mensaje como leído
        public async Task MarcarLeidoAsync(string id)
        {
            var update = Builders<Mensaje>.Update.Set(m => m.Leido, true);
            await _collection.UpdateOneAsync(m => m.Id == id, update);
        }

        public async Task EliminarAsync(string id)
        {
            await _collection.DeleteOneAsync(m => m.Id == id);
        }

        public async Task EliminarPorChatsAsync(List<string> chatIds)
        {
            await _collection.DeleteManyAsync(m => chatIds.Contains(m.ChatId));
        }
    }
}