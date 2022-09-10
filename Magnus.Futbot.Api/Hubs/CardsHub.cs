﻿using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Hubs
{
    public class CardsHub : Hub<ICardsClient>
    {
        private readonly CardsCache _cardsCache;

        public CardsHub(CardsCache cardsCache)
        {
            _cardsCache = cardsCache;
        }

        public async Task GetCards()
            => await Clients.All.OnCardsLoaded(_cardsCache.Cards);

        public async Task BuyCard(BuyCardDTO buyCardDTO)
        {

        }
    }
}
