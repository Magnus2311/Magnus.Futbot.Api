using Magnus.Futbot.Common.Models.Database.Card;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;
using MongoDB.Driver;

namespace Magnus.Futbot.Api.Services
{
    public class DatabaseIndexService
    {
        private readonly CardsRepository _cardsRepository;
        private readonly PlayersRepository _playersRepository;

        public DatabaseIndexService(CardsRepository cardsRepository, PlayersRepository playersRepository)
        {
            _cardsRepository = cardsRepository;
            _playersRepository = playersRepository;
        }

        public async Task CreateOptimizedIndexesAsync()
        {
            Console.WriteLine("Checking and creating database indexes for optimal performance...");

            // Create indexes for Cards collection
            await CreateCardsIndexes();

            // Create indexes for Players collection  
            await CreatePlayersIndexes();

            Console.WriteLine("Database indexes verified/created successfully!");
        }

        private async Task CreateCardsIndexes()
        {
            var cardsCollection = _cardsRepository.GetCollection();

            // Index for EAId lookups (most common query)
            await CreateIndexIfNotExistsAsync(cardsCollection,
                new CreateIndexModel<Card>(
                    Builders<Card>.IndexKeys.Ascending(c => c.EAId),
                    new CreateIndexOptions { Name = "IX_Cards_EAId" }
                ), "IX_Cards_EAId");

            // Index for OverallRating sorting
            await CreateIndexIfNotExistsAsync(cardsCollection,
                new CreateIndexModel<Card>(
                    Builders<Card>.IndexKeys.Descending(c => c.OverallRating),
                    new CreateIndexOptions { Name = "IX_Cards_OverallRating" }
                ), "IX_Cards_OverallRating");

            // Compound index for common queries
            await CreateIndexIfNotExistsAsync(cardsCollection,
                new CreateIndexModel<Card>(
                    Builders<Card>.IndexKeys
                        .Ascending(c => c.IsDeleted)
                        .Descending(c => c.OverallRating),
                    new CreateIndexOptions { Name = "IX_Cards_IsDeleted_OverallRating" }
                ), "IX_Cards_IsDeleted_OverallRating");
        }

        private async Task CreatePlayersIndexes()
        {
            var playersCollection = _playersRepository.GetCollection();

            // Index for Player ID lookups
            await CreateIndexIfNotExistsAsync(playersCollection,
                new CreateIndexModel<PlayerDocument>(
                    Builders<PlayerDocument>.IndexKeys.Ascending(p => p.Id),
                    new CreateIndexOptions { Name = "IX_Players_Id" }
                ), "IX_Players_Id");
        }

        private async Task CreateIndexIfNotExistsAsync<T>(IMongoCollection<T> collection, CreateIndexModel<T> indexModel, string indexName)
        {
            try
            {
                // Check if index already exists
                var existingIndexes = await collection.Indexes.ListAsync();
                var indexList = await existingIndexes.ToListAsync();

                var indexExists = indexList.Any(index => index["name"].AsString == indexName);

                if (!indexExists)
                {
                    await collection.Indexes.CreateOneAsync(indexModel);
                    Console.WriteLine($"Created index: {indexName}");
                }
                else
                {
                    Console.WriteLine($"Index already exists: {indexName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating index {indexName}: {ex.Message}");
                // Continue with other indexes even if one fails
            }
        }
    }
}
