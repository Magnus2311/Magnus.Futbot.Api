using Magnus.Futbot.Database.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Magnus.Futbot.Database.Repositories
{
    public class SuccessfulPurchaseRepository : BaseRepository<SuccessfulPurchase>
    {
        public SuccessfulPurchaseRepository(IConfiguration configuration) : base(configuration)
        {
        }

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

        public async Task<IEnumerable<SuccessfulPurchase>> GetByCardIdAsync(string cardId)
        {
            return await (await _collection.FindAsync(sp => sp.Card.CardId == cardId && !sp.IsDeleted)).ToListAsync();
        }
    }
}
