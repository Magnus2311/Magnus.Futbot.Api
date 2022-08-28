using Magnus.Futbot.Database.Models;
using Microsoft.Extensions.Configuration;

namespace Magnus.Futbot.Database.Repositories
{
    public class RevisionsRepository : BaseRepository<Revision>
    {
        public RevisionsRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
