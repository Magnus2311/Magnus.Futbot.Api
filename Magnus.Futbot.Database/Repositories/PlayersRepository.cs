using Magnus.Futbot.Database.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Magnus.Futbot.Database.Repositories
{
    public class PlayersRepository : DatabaseContext
    {
        private readonly IMongoCollection<PlayerDocument> _collection;

        public PlayersRepository(IConfiguration configuration) : base(configuration)
        {
            _collection = _db.GetCollection<PlayerDocument>("Players");

            var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<PlayerDocument>>().Match("{ operationType: { $in: [ 'insert', 'delete' ] } }");

            Cursor = _collection.Watch<ChangeStreamDocument<PlayerDocument>>(pipeline, options);
        }

        public IChangeStreamCursor<ChangeStreamDocument<PlayerDocument>> Cursor { get; }

        public async Task AddPlayers(IEnumerable<PlayerDocument> players)
            => await _collection.InsertManyAsync(players);

        public async Task<IEnumerable<PlayerDocument>> GetAll()
            => (await _collection.FindAsync(FilterDefinition<PlayerDocument>.Empty)).ToList();
    }
}