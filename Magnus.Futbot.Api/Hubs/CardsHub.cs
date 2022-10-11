using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services.Interfaces;
using Magnus.Futbot.Common.Models.Database.Card;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;

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

        public async Task GetCards(string name)
            => await Clients.Client(Context.ConnectionId).OnCardsLoaded(_cardsCache.Cards.Where(c => c.Name.ToUpperInvariant().Contains(name.ToUpperInvariant())).Take(20));
        
        public Card? GetCardById(string cardId)
            => _cardsCache?.Cards?.FirstOrDefault(c => c.Id == new ObjectId(cardId));

        public Task BuyCard(BuyCardDTO buyCardDTO)
            => _tradingService.Buy(buyCardDTO);

        public Task BuyAndSell(BuyAndSellCardDTO buyAndSellCardDTO)
            => _tradingService.BuyAndSell(buyAndSellCardDTO);

        public Task SellCard(SellCardDTO sellCardDTO)
            => _tradingService.Sell(sellCardDTO);

        public Task SendTransferTargetsToTransferList(string email)
            => _tradingService.MoveCardsFromTransferTargetsToTransferList(email);

        public Task SendUnassignedItemsToTransferList(string email)
            => _tradingService.SendUnassignedItemsToTransferList(email);
    }
}
