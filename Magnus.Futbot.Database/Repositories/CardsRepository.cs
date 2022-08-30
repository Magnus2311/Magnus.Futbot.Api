using Magnus.Futbot.Database.Models.Card;
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
    }
}
