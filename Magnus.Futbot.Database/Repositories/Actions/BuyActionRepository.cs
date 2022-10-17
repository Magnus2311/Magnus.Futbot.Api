﻿using Magnus.Futbot.Database.Models.Actions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Magnus.Futbot.Database.Repositories.Actions
{
    public class BuyActionRepository : DatabaseContext
    {
        private readonly IMongoCollection<BuyActionEntity> _collection;

        public BuyActionRepository(IConfiguration configuration) : base(configuration)
        {
            _collection = _db.GetCollection<BuyActionEntity>("BuyActions");
        }

        public async Task<IEnumerable<BuyActionEntity>> GetActionsByProfileId(ObjectId profileId)
            => await (await _collection.FindAsync(e => !e.IsDeleted && e.ProfileId == profileId)).ToListAsync();

        public Task Add(IEnumerable<BuyActionEntity> entities)
            => _collection.InsertManyAsync(entities);

        public Task Add(BuyActionEntity entity)
            => _collection.InsertOneAsync(entity);

        public Task Update(BuyActionEntity entity)
        {
            entity.UpdatedDate = DateTime.Now;

            return _collection.ReplaceOneAsync(Builders<BuyActionEntity>.Filter.Eq(e => e.Id, entity.Id), entity);
        }

        public async Task<BuyActionEntity> GetById(ObjectId actionId)
            => await (await _collection.FindAsync(e => !e.IsDeleted && e.Id == actionId)).FirstOrDefaultAsync();
    }
}