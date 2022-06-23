using Magnus.Futbot.Database.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Magnus.Futbot.Database.Repositories
{
    public abstract class BaseRepository<TEntity> : DatabaseContext
        where TEntity : class, IEntity
    {
        protected readonly IMongoCollection<TEntity> _collection;

        public BaseRepository(IConfiguration configuration) : base(configuration)
        {
            _collection = _db.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            entity.CreatedDate = DateTime.Now;
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task Delete(ObjectId id)
        {
            var entity = await (await _collection.FindAsync(e => e.Id == id)).FirstOrDefaultAsync();
            entity.IsDeleted = true;
            await _collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id), entity);

        }

        public async Task<TEntity> Recover(ObjectId id)
        {
            var entity = await (await _collection.FindAsync(e => e.Id == id)).FirstOrDefaultAsync();
            entity.IsDeleted = false;
            await _collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id), entity);
            return entity;
        }

        public async Task<TEntity> Get(ObjectId id, ObjectId userId)
            => await (await _collection.FindAsync(e => e.Id == id && e.UserId == userId)).FirstOrDefaultAsync();

        public async Task<IEnumerable<TEntity>> GetActive(ObjectId userId)
            => await (await _collection.FindAsync(e => !e.IsDeleted && e.UserId == userId)).ToListAsync();

        public async Task<IEnumerable<TEntity>> GetDeleted(ObjectId userId)
            => await (await _collection.FindAsync(e => e.IsDeleted && e.UserId == userId)).ToListAsync();

        public async Task<IEnumerable<TEntity>> GetAll(ObjectId userId)
            => await (await _collection.FindAsync(e => e.UserId == userId)).ToListAsync();

        public async Task<IEnumerable<TEntity>> GetAll()
            => await (await _collection.FindAsync(FilterDefinition<TEntity>.Empty)).ToListAsync();

        public async Task Update(TEntity entity)
        {
            var oldEntity = await (await _collection.FindAsync(e => e.Id == entity.Id)).FirstOrDefaultAsync();
            if (oldEntity.UserId == entity.UserId)
            {
                entity.UpdatedDate = DateTime.Now;

                await _collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id), entity);
            }
        }
    }
}
