using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace BTSharedCore.Services
{
    public class Mongo
    {
        public class DatabaseSettings
        {
            public string ConnectionString { get; set; }
            public string CollectionName { get; set; }
        }

        private readonly IMongoDatabase _database;

        public Mongo(IOptions<DatabaseSettings> config, ILogger<IMongoClient> logger)
        {
            var databaseSettings = config.Value;

            var mongoConnectionUrl = new MongoUrl(databaseSettings.ConnectionString);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);
            mongoClientSettings.MaxConnectionPoolSize = 60;

            mongoClientSettings.ClusterConfigurator = cb =>
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    logger.LogDebug($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };

            var mongoClient = new MongoClient(mongoClientSettings);
            _database = mongoClient.GetDatabase(databaseSettings.CollectionName);
        }

        public IMongoCollection<T> Collection<T>(string collectionName = null)
        {
            collectionName ??= typeof(T).Name;  

            return _database.GetCollection<T>(collectionName);
        }
    }
}
