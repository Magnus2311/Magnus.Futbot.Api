using Magnus.Futbot.Database.Models.Actions;
using Microsoft.Extensions.Configuration;

namespace Magnus.Futbot.Database.Repositories.Actions
{
    public class MoveActionRepository : ActionsRepository<MoveActionEntity>
    {
        public MoveActionRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
