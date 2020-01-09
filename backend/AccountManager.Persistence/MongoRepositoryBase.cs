using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AccountManager.Persistence
{
    public abstract class MongoRepositoryBase<T> where T : MongoEntityBase
    {
        protected MongoRepositoryBase(AMMongoContext context)
        {
            Context = context;
        }

        protected AMMongoContext Context { get; }

        protected abstract string CollectionName { get; }
        public async Task<T> GetAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            return (await Context.GetCollection<T>(CollectionName).FindAsync(filter)).SingleOrDefault();
        }

        public async Task AddAsync(T entity)
        {
            await Context.GetCollection<T>(CollectionName).InsertOneAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", entity.Id);
            await Context.GetCollection<T>(CollectionName).DeleteOneAsync(filter);
        }

        public IMongoCollection<T> GetCollection()
        {
            return Context.GetCollection<T>(CollectionName);
        }
    }
}
