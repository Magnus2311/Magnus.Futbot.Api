using Magnus.Futbot.Database.Models;
using Microsoft.Extensions.Configuration;

namespace Magnus.Futbot.Database.Repositories
{
    public class TradesRepository : BaseRepository<Trade>
    {
        public TradesRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
