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
        private readonly SemaphoreSlim _initializationLock = new(1, 1);
        private volatile bool _isInitialized = false;

        public CardsCache(CardsRepository cardsRepository,
            CardsConnection cardsConnection)
        {
            _cardsRepository = cardsRepository;
            _cardsConnection = cardsConnection;
            Cards = new ConcurrentBag<Card>();
        }

        public ConcurrentBag<Card> Cards { get; private set; }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            await _initializationLock.WaitAsync();
            try
            {
                if (_isInitialized) return;

                Console.WriteLine("Starting CardsCache initialization...");
                var startTime = DateTime.UtcNow;

                // Use streaming approach for better memory management
                await LoadCardsInBatches();

                _isInitialized = true;
                var duration = DateTime.UtcNow - startTime;
                Console.WriteLine($"CardsCache initialized with {Cards.Count} cards in {duration.TotalSeconds:F2} seconds");

                // Start the change stream listener
                _ = Task.Run(OnCardAdded);
            }
            finally
            {
                _initializationLock.Release();
            }
        }

        private async Task LoadCardsInBatches(int batchSize = 1000)
        {
            var skip = 0;
            var hasMore = true;

            while (hasMore)
            {
                var batch = await _cardsRepository.GetAllPagedAsync(skip, batchSize);
                var batchList = batch.ToList();

                if (!batchList.Any())
                {
                    hasMore = false;
                    break;
                }

                foreach (var card in batchList)
                {
                    Cards.Add(card);
                }

                skip += batchSize;
                Console.WriteLine($"Loaded {skip} cards...");
            }
        }

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
