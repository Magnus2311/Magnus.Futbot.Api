using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;

namespace Magnus.Futbot.Api.Services
{
    public class PriceService(PriceRepository priceRepository)
    {
        private readonly PriceRepository _priceRepository = priceRepository;

        public Task Add(PlayerPrice playerPrice)
        {
            return _priceRepository.AddOrUpdate(playerPrice);
        }

        public Task<PlayerPrice> Get(string cardId)
        {
            return _priceRepository.Get(cardId);
        }

        public Task<IEnumerable<PlayerPrice>> Get(List<string> cardIds)
        {
            return _priceRepository.Get(cardIds);
        }
    }
}
