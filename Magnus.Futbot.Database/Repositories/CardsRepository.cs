using Magnus.Futbot.Database.Models.Card;
using Microsoft.Extensions.Configuration;

namespace Magnus.Futbot.Database.Repositories
{
    public class CardsRepository : BaseRepository<Card>
    {
        public CardsRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
