using Magnus.Futbot.Database.Models.Actions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Magnus.Futbot.Database.Repositories.Actions
{
    public class SellActionRepository : DatabaseContext
    {
        private readonly IMongoCollection<SellActionEntity> _collection;

        public SellActionRepository(IConfiguration configuration) : base(configuration)
        {
            _collection = _db.GetCollection<SellActionEntity>("SellActions");
        }

        public Task Add(IEnumerable<SellActionEntity> entities)
            => _collection.InsertManyAsync(entities);

        public Task Add(SellActionEntity entity)
            => _collection.InsertOneAsync(entity);

        public async Task<IEnumerable<SellActionEntity>> GetActionsByProfileId(ObjectId profileId)
            => await (await _collection.FindAsync(e => !e.IsDeleted && e.ProfileId == profileId)).ToListAsync();

        public Task Update(SellActionEntity entity)
        {
            entity.UpdatedDate = DateTime.Now;

            return _collection.ReplaceOneAsync(Builders<SellActionEntity>.Filter.Eq(e => e.Id, entity.Id), entity);
        }

        public async Task<SellActionEntity> GetById(ObjectId actionId)
            => await (await _collection.FindAsync(e => !e.IsDeleted && e.Id == actionId)).FirstOrDefaultAsync();

        public Task DeactivateAllActions()
            => _collection.UpdateManyAsync(Builders<SellActionEntity>.Filter.Empty, Builders<SellActionEntity>.Update.Inc(e => e.IsDeleted, true));
    }
}
