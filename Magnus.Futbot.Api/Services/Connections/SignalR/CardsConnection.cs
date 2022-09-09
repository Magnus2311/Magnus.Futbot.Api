using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Magnus.Futbot.Common.Models.Database.Card;

namespace Magnus.Futbot.Api.Services.Connections.SignalR
{
    public class CardsConnection
    {
        private readonly IHubContext<CardsHub, ICardsClient> _cardsHub;

        public CardsConnection(IHubContext<CardsHub, ICardsClient> cardsHub)
        {
            _cardsHub = cardsHub;
        }

        public async Task AddCard(Card card)
            => await _cardsHub.Clients.All.OnCardAdded(card);
    }
}
