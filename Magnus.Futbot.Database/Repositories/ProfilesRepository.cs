using Magnus.Futbot.Database.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Magnus.Futbot.Database.Repositories
{
    public class ProfilesRepository : BaseRepository<ProfileDocument>
    {
        public ProfilesRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<ProfileDocument>> GetByEmail(string email)
            => await (await _collection.FindAsync(pd => pd.Email.ToUpperInvariant() == email.ToUpperInvariant())).ToListAsync();
    }
}
