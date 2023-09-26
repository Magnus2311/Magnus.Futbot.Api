using Magnus.Futbot.Database.Models.Actions;
using Microsoft.Extensions.Configuration;

namespace Magnus.Futbot.Database.Repositories.Actions
{
    public class PauseActionRepository : ActionsRepository<PauseActionEntity>
    {
        public PauseActionRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
