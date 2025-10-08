using Magnus.Futbot.Database.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Magnus.Futbot.Database.Repositories
{
    public class SuccessfulPurchaseRepository(IConfiguration configuration) : BaseRepository<SuccessfulPurchase>(configuration)
    {
        public async Task<IEnumerable<SuccessfulPurchase>> GetByPidIdAsync(string pidId)
        {
            return await (await _collection.FindAsync(sp => sp.PidId == pidId && !sp.IsDeleted)).ToListAsync();
        }

        public async Task<IEnumerable<SuccessfulPurchase>> GetByPidIdAndDateRangeAsync(string pidId, DateTime startDate, DateTime endDate)
        {
            return await (await _collection.FindAsync(sp =>
                sp.PidId == pidId &&
                !sp.IsDeleted &&
                sp.PurchaseDate >= startDate &&
                sp.PurchaseDate <= endDate)).ToListAsync();
        }

        public async Task<long> GetCountByPidIdAsync(string pidId)
        {
            return await _collection.CountDocumentsAsync(sp => sp.PidId == pidId && !sp.IsDeleted);
        }

        public async Task<IEnumerable<SuccessfulPurchase>> GetFilteredPurchasesAsync(string pidId, string? position = null, string? quality = null, string? league = null, string? club = null)
        {
            var filter = Builders<SuccessfulPurchase>.Filter.And(
                Builders<SuccessfulPurchase>.Filter.Eq(sp => sp.PidId, pidId),
                Builders<SuccessfulPurchase>.Filter.Eq(sp => sp.IsDeleted, false),
                Builders<SuccessfulPurchase>.Filter.Ne(sp => sp.Filters, null)
            );

            if (!string.IsNullOrEmpty(position))
                filter = Builders<SuccessfulPurchase>.Filter.And(filter, Builders<SuccessfulPurchase>.Filter.Eq("Filters.Position", position));

            if (!string.IsNullOrEmpty(quality))
                filter = Builders<SuccessfulPurchase>.Filter.And(filter, Builders<SuccessfulPurchase>.Filter.Eq("Filters.Quality", quality));

            if (!string.IsNullOrEmpty(league))
                filter = Builders<SuccessfulPurchase>.Filter.And(filter, Builders<SuccessfulPurchase>.Filter.Eq("Filters.League", league));

            if (!string.IsNullOrEmpty(club))
                filter = Builders<SuccessfulPurchase>.Filter.And(filter, Builders<SuccessfulPurchase>.Filter.Eq("Filters.Club", club));

            return await (await _collection.FindAsync(filter)).ToListAsync();
        }

        public async Task<long> GetFilteredPurchasesCountAsync(string pidId)
        {
            return await _collection.CountDocumentsAsync(sp =>
                sp.PidId == pidId &&
                !sp.IsDeleted &&
                sp.Filters != null);
        }
    }
}
