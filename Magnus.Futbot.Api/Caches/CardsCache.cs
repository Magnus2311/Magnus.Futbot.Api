using Magnus.Futbot.Api.Services.Connections.SignalR;
using Magnus.Futbot.Database.Models.Card;
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

            Cards = new ConcurrentBag<Card>(_cardsRepository.GetAll().Result.OrderByDescending(c => c.Rating));
            Task.Run(async () =>
            {
                await OnCardAdded();
            });
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
                Cards = new ConcurrentBag<Card>(Cards.OrderByDescending(c => c.Rating));
                await _cardsConnection.AddCard(card);
            }
        }
    }
}
