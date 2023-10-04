using Magnus.Futbot.Database.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Magnus.Futbot.Database.Repositories.Actions
{
    public abstract class ActionsRepository<TActionEntity> : DatabaseContext 
        where TActionEntity : IActionEntity
    {
        private readonly IMongoCollection<TActionEntity> _collection;

        public ActionsRepository(IConfiguration configuration) : base(configuration)
        {
            _collection = _db.GetCollection<TActionEntity>(typeof(TActionEntity).Name);
        }

        public async Task<IEnumerable<TActionEntity>> GetActionsByProfileId(ObjectId profileId)
            => await (await _collection.FindAsync(e => !e.IsDeleted && e.ProfileId == profileId)).ToListAsync();

        public async Task<IEnumerable<TActionEntity>> GetAllActive()
            => await (await _collection.FindAsync(e => !e.IsDeleted)).ToListAsync();

        public Task DeactivateAllActions()
            => _collection.UpdateManyAsync(Builders<TActionEntity>.Filter.Empty, Builders<TActionEntity>.Update.Set(e => e.IsDeleted, true));

        public Task Add(IEnumerable<TActionEntity> entities)
            => _collection.InsertManyAsync(entities);

        public Task Add(TActionEntity entity)
            => _collection.InsertOneAsync(entity);

        public Task Update(TActionEntity entity)
        {
            entity.UpdatedDate = DateTime.Now;

            return _collection.ReplaceOneAsync(Builders<TActionEntity>.Filter.Eq(e => e.Id, entity.Id), entity);
        }

        public async Task<TActionEntity> GetById(ObjectId actionId)
            => await (await _collection.FindAsync(e => !e.IsDeleted && e.Id == actionId)).FirstOrDefaultAsync();

        public Task DeactivateById(ObjectId actionId)
            => _collection.UpdateOneAsync(Builders<TActionEntity>.Filter.Eq(a => a.Id, actionId), Builders<TActionEntity>.Update.Set(e => e.IsDeleted, true));

        public Task DeactivateAllActions(ObjectId profileId)
            => _collection.UpdateOneAsync(Builders<TActionEntity>.Filter.Eq(a => a.ProfileId, profileId), Builders<TActionEntity>.Update.Set(e => e.IsDeleted, true));
    }
}
