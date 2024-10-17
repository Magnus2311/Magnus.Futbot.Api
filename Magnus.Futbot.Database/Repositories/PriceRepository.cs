using Magnus.Futbot.Database.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Magnus.Futbot.Database.Repositories
{
    public class PriceRepository : DatabaseContext
    {
        private readonly IMongoCollection<PlayerPrice> _collection;
        private readonly IMemoryCache _cache;  
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(1);

        public PriceRepository(IConfiguration configuration, IMemoryCache cache) : base(configuration)
        {
            _collection = _db.GetCollection<PlayerPrice>("PlayerPrices");
            _cache = cache;
        }

        public async Task AddOrUpdate(PlayerPrice playerPrice)
        {
            var filter = Builders<PlayerPrice>.Filter.Eq(p => p.CardId, playerPrice.CardId);
            var update = Builders<PlayerPrice>.Update.PushEach(p => p.Prices, playerPrice.Prices);

            await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });

            _cache.Set(playerPrice.CardId, playerPrice, _cacheExpiration);
        }

        public async Task<PlayerPrice> Get(string cardId)
        {
            if (_cache.TryGetValue(cardId, out PlayerPrice cachedPlayerPrice))
                return cachedPlayerPrice;

            var playerPriceFromDb = await _collection.Find(p => p.CardId == cardId)
                .Project(p => new PlayerPrice
                {
                    CardId = p.CardId,
                    Prices = p.Prices.OrderByDescending(price => price.CreatedDate).Take(5).ToList()
                })
                .FirstOrDefaultAsync();

            if (playerPriceFromDb != null)
                _cache.Set(cardId, playerPriceFromDb, _cacheExpiration);

            return playerPriceFromDb;
        }
    }
}
