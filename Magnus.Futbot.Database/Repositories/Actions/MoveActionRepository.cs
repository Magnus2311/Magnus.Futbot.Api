using Magnus.Futbot.Database.Models.Actions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Magnus.Futbot.Database.Repositories.Actions
{
    public class MoveActionRepository : DatabaseContext
    {
        private readonly IMongoCollection<MoveActionEntity> _collection;

        public MoveActionRepository(IConfiguration configuration) : base(configuration)
        {
            _collection = _db.GetCollection<MoveActionEntity>("MoveActions");
        }

        public async Task<IEnumerable<MoveActionEntity>> GetActionsByProfileId(ObjectId profileId)
            => await (await _collection.FindAsync(e => !e.IsDeleted && e.ProfileId == profileId)).ToListAsync();

        public Task Add(IEnumerable<MoveActionEntity> entities)
            => _collection.InsertManyAsync(entities);

        public Task Add(MoveActionEntity entity)
            => _collection.InsertOneAsync(entity);

        public Task Update(MoveActionEntity entity)
        {
            entity.UpdatedDate = DateTime.Now;

            return _collection.ReplaceOneAsync(Builders<MoveActionEntity>.Filter.Eq(e => e.Id, entity.Id), entity);
        }

        public async Task<MoveActionEntity> GetById(ObjectId actionId)
            => await (await _collection.FindAsync(e => !e.IsDeleted && e.Id == actionId)).FirstOrDefaultAsync();

        public Task DeactivateAllActions()
            => _collection.UpdateManyAsync(Builders<MoveActionEntity>.Filter.Empty, Builders<MoveActionEntity>.Update.Set(e => e.IsDeleted, true));

        public async Task DeactivateById(ObjectId actionId)
        {
            var action = await GetById(actionId);
            action.IsDeleted = true;
            await Update(action);
        }
    }
}
