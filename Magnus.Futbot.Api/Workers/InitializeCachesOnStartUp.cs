using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Api.Services;

namespace Magnus.Futbot.Api.Workers
{
    public class InitializeCachesOnStartUp : BackgroundService
    {
        private readonly CardsCache _cardsCache;
        private readonly PlayersCache _playersCache;
        private readonly DatabaseIndexService _databaseIndexService;

        public InitializeCachesOnStartUp(CardsCache cardsCache, PlayersCache playersCache, DatabaseIndexService databaseIndexService)
        {
            _cardsCache = cardsCache;
            _playersCache = playersCache;
            _databaseIndexService = databaseIndexService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Initialize caches asynchronously to avoid blocking startup
            Console.WriteLine("Starting application initialization...");

            // Create database indexes first for optimal performance
            await _databaseIndexService.CreateOptimizedIndexesAsync();

            // Initialize CardsCache asynchronously with batching
            await _cardsCache.InitializeAsync();

            // Initialize PlayersCache (still using the old synchronous approach for now)
            var players = _playersCache.Players;
            Console.WriteLine($"PlayersCache initialized with {players.Count} players");

            Console.WriteLine("All caches initialized successfully!");
        }
    }
}
