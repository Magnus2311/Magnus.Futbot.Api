using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services.Interfaces;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Hubs
{
    public class CardsHub : Hub<ICardsClient>
    {
        private readonly CardsCache _cardsCache;
        private readonly ITradingService _tradingService;

        public CardsHub(CardsCache cardsCache,
            ITradingService tradingService)
        {
            _cardsCache = cardsCache;
            _tradingService = tradingService;
        }

        public async Task GetCards()
            => await Clients.All.OnCardsLoaded(_cardsCache.Cards);

        public async Task BuyCard(BuyCardDTO buyCardDTO)
        {
            await _tradingService.Buy(buyCardDTO);
        }

        public async Task SellCard(SellCardDTO sellCardDTO)
        {

        }
    }
}
