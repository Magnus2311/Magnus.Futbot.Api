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

        public Task BuyCard(BuyCardDTO buyCardDTO)
            => _tradingService.Buy(buyCardDTO);

        public Task SellCard(SellCardDTO sellCardDTO)
            => _tradingService.Sell(sellCardDTO);

        public Task SendTransferTargetsToTransferList(string email)
            => _tradingService.MoveCardsFromTransferTargetsToTransferList(email);
    }
}
