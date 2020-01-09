using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Audit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace AccountManager.Persistence
{
    public class AMMongoContext : IAMMongoContext
    {
        private readonly IMongoDatabase _database = null;

        public AMMongoContext(MongoContextSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.Database);

            if (!BsonClassMap.IsClassMapRegistered(typeof(AuditLog)))
                BsonClassMap.RegisterClassMap<MongoEntityBase>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(c => c.Id)
                        .SetIgnoreIfDefault(true)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                });
        }

        public IMongoCollection<AuditLog> AuditLogs => _database.GetCollection<AuditLog>("AuditLogs");

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }

    public class MongoContextSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
