using Magnus.Futbot.Api.Services.Connections.SignalR;
using Magnus.Futbot.Common.Models.Database.Card;
using Magnus.Futbot.Database.Repositories;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace Magnus.Futbot.Api.Caches
{
    public class CardsCache
    {
        private readonly CardsRepository _cardsRepository;
        private readonly CardsConnection _cardsConnection;

        public CardsCache(CardsRepository cardsRepository,
            CardsConnection cardsConnection)
        {
            _cardsRepository = cardsRepository;
            _cardsConnection = cardsConnection;

            Cards = [.. _cardsRepository.GetAll().Result];
            Task.Run(OnCardAdded);
        }

        public ConcurrentBag<Card> Cards { get; private set; }

        private async Task OnCardAdded()
        {
            var enumerator = _cardsRepository.Cursor.ToEnumerable().GetEnumerator();
            while (enumerator.MoveNext())
            {
                var card = enumerator.Current.FullDocument;

                Cards.Add(card);

                // Should be optimised somehow
                Cards = [.. Cards.OrderByDescending(c => c.OverallRating)];
                await _cardsConnection.AddCard(card);
            }
        }
    }
}
