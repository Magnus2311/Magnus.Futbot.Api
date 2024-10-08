﻿using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services.Interfaces;
using Magnus.Futbot.Common.Models.Database.Card;
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

        public async Task GetCards(string name)
            => await Clients.Client(Context.ConnectionId).OnCardsLoaded(_cardsCache
                .Cards
                .Where(c => c.Name.ToUpperInvariant().Contains(name.ToUpperInvariant()))
                .OrderByDescending(c => c.OverallRating)
                .GroupBy(c => new { c.Name, c.OverallRating })
                .Select(g => g.First()) 
                .Take(20));
        
        public Card? GetCardById(long cardId)
            => _cardsCache?.Cards?.FirstOrDefault(c => c.EAId == cardId);

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

        public Task RelistAllForProfile(string email)
            => _tradingService.RelistPlayersByProfile(email);

        public Task ClearSoldItemsForProfile(string email)
            => _tradingService.ClearSoldCards(email);
    }
}
