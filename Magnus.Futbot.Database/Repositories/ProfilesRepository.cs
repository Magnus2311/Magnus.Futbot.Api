using Magnus.Futbot.Common;
using Magnus.Futbot.Database.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
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

        public async Task<IEnumerable<ProfileDocument>> GetById(ObjectId id)
            => await (await _collection.FindAsync(pd => pd.Id == id)).ToListAsync();

        public async Task<IEnumerable<ProfileDocument>> GetAutoRelistProfiles()
            => await (await _collection.FindAsync(e => e.AutoRelist == true)).ToListAsync();

        public async Task<ProfileDocument> UpdateSubmitCodeStatus(string email, ConfirmationCodeStatusType confirmationCodeStatusType)
        {
            var profile = await (await _collection.FindAsync(pd => pd.Email.ToUpperInvariant() == email.ToUpperInvariant())).FirstOrDefaultAsync();
            if (confirmationCodeStatusType == ConfirmationCodeStatusType.Successful)
            {
                profile.ProfilesStatus = ProfileStatusType.Logged;
                await Update(profile);
            }

            return profile;
        }
    }
}
