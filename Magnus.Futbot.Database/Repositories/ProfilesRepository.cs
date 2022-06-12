using Magnus.Futbot.Common;
using Magnus.Futbot.Database.Models;
using Microsoft.Extensions.Configuration;

namespace Magnus.Futbot.Database.Repositories
{
    public class ProfilesRepository : BaseRepository<ProfileDocument>
    {
        public ProfilesRepository(IConfiguration configuration, AppSettings appSettings) : base(configuration, appSettings)
        {
        }
    }
}
