using MongoDB.Driver;
using VooApi.Database;
using VooApi.Models;

namespace VooApi.Data
{
    public class RetoCompletadoRepository
    {
        private readonly IMongoCollection<RetoCompletado> _collection;

        public RetoCompletadoRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<RetoCompletado>("retos_completados");
        }

        public async Task<bool> YaCompletadoAsync(string retoId, string usuarioId)
        {
            return await _collection
                .Find(x => x.RetoId == retoId && x.UsuarioId == usuarioId)
                .AnyAsync();
        }

        public async Task InsertarAsync(RetoCompletado completado)
        {
            await _collection.InsertOneAsync(completado);
        }

        public async Task EliminarPorUsuarioAsync(string usuarioId)
        {
            await _collection.DeleteManyAsync(r =>
                r.UsuarioId == usuarioId || r.UsuarioEscaneadoId == usuarioId
            );
        }
    }
}