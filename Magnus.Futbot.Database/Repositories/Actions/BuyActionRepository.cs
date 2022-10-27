using Magnus.Futbot.Database.Models.Actions;
using Microsoft.Extensions.Configuration;

namespace Magnus.Futbot.Database.Repositories.Actions
{
    public class BuyActionRepository : ActionsRepository<BuyActionEntity>
    {
        public BuyActionRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
