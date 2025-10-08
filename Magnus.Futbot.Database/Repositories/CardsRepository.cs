using Magnus.Futbot.Common.Models.Database.Card;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Magnus.Futbot.Database.Repositories
{
    public class CardsRepository : BaseRepository<Card>
    {
        public CardsRepository(IConfiguration configuration) : base(configuration)
        {
            var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Card>>().Match("{ operationType: { $in: [ 'insert', 'delete' ] } }");

            Cursor = _collection.Watch(pipeline, options);
        }

        public IChangeStreamCursor<ChangeStreamDocument<Card>> Cursor { get; }

        public async Task<HashSet<int>> GetExistingEaIdsAsync(IEnumerable<int> eaIds)
        {
            var filter = Builders<Card>.Filter.In(c => c.EAId, eaIds);
            var existing = await _collection.Find(filter).ToListAsync();
            return existing.Select(c => c.EAId).ToHashSet();
        }

        public async Task<IEnumerable<Card>> GetAllPagedAsync(int skip, int limit)
        {
            return await _collection
                .Find(FilterDefinition<Card>.Empty)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();
        }

        public IMongoCollection<Card> GetCollection() => _collection;
    }
}
