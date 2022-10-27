using Magnus.Futbot.Database.Models.Actions;
using Microsoft.Extensions.Configuration;

namespace Magnus.Futbot.Database.Repositories.Actions
{
    public class SellActionRepository : ActionsRepository<SellActionEntity>
    {
        public SellActionRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
