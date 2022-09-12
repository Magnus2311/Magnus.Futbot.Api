using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Common.Interfaces.Helpers;
using Magnus.Futbot.Common.Models.Database.Card;

namespace Magnus.Futbot.Api.Services.Helpers
{
    public class CardsHelper : ICardsHelper
    {
        private readonly CardsCache _cardsCache;

        public CardsHelper(CardsCache cardsCache)
        {
            _cardsCache = cardsCache;
        }

        public IEnumerable<Card> GetAllCards()
            => _cardsCache.Cards;
    }
}
