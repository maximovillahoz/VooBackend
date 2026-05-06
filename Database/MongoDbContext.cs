using MongoDB.Driver;

namespace VooApi.Database
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration config)
        {
            var connectionString = config["MongoDB:ConnectionString"];
            var databaseName = config["MongoDB:DatabaseName"];

            Console.WriteLine("========== MONGO CONFIG ==========");
            Console.WriteLine($"ConnectionString: {connectionString}");
            Console.WriteLine($"DatabaseName: {databaseName}");
            Console.WriteLine("==================================");

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);

            Console.WriteLine($"Mongo conectado a DB: {_database.DatabaseNamespace.DatabaseName}");
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            Console.WriteLine($"Usando colección: {name}");
            return _database.GetCollection<T>(name);
        }
    }
}