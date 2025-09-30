using Magnus.Futbot.Database.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Magnus.Futbot.Database.Repositories
{
    public class PlayerListRepository : DatabaseContext
    {
        private readonly IMongoCollection<PlayerList> _playerLists;

        public PlayerListRepository(IConfiguration configuration) : base(configuration)
        {
            _playerLists = _db.GetCollection<PlayerList>("playerlists");
        }

        public async Task<List<PlayerList>> GetByPidIdAsync(string pidId)
        {
            return await _playerLists.Find(x => x.PidId == pidId).ToListAsync();
        }

        public async Task<PlayerList?> GetByIdAsync(string id)
        {
            return await _playerLists.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<PlayerList> CreateAsync(PlayerList playerList)
        {
            await _playerLists.InsertOneAsync(playerList);
            return playerList;
        }

        public async Task<PlayerList> UpdateAsync(PlayerList playerList)
        {
            playerList.UpdatedDate = DateTime.Now;
            await _playerLists.ReplaceOneAsync(x => x.Id == playerList.Id, playerList);
            return playerList;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _playerLists.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> AddPlayerToListAsync(string listId, string playerId)
        {
            var update = Builders<PlayerList>.Update
                .AddToSet(x => x.PlayerIds, playerId)
                .Set(x => x.UpdatedDate, DateTime.Now);

            var result = await _playerLists.UpdateOneAsync(x => x.Id == listId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemovePlayerFromListAsync(string listId, string playerId)
        {
            var update = Builders<PlayerList>.Update
                .Pull(x => x.PlayerIds, playerId)
                .Set(x => x.UpdatedDate, DateTime.Now);

            var result = await _playerLists.UpdateOneAsync(x => x.Id == listId, update);
            return result.ModifiedCount > 0;
        }
    }
}
