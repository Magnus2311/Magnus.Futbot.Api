using Magnus.Futbot.Api.Models.Requests;
using Magnus.Futbot.Api.Models.Responses;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;

namespace Magnus.Futbot.Api.Services
{
    public class PlayerListService(PlayerListRepository playerListRepository)
    {
        public async Task<List<PlayerListResponse>> GetByPidIdAsync(string pidId)
        {
            var playerLists = await playerListRepository.GetByPidIdAsync(pidId);
            return [.. playerLists.Select(pl => new PlayerListResponse
            {
                Id = pl.Id,
                Name = pl.Name,
                PidId = pl.PidId,
                CreatedDate = pl.CreatedDate,
                UpdatedDate = pl.UpdatedDate,
                PlayerIds = pl.PlayerIds,
                PlayerCount = pl.PlayerIds.Count
            })];
        }

        public async Task<PlayerListResponse> GetByIdAsync(string id)
        {
            var playerList = await playerListRepository.GetByIdAsync(id);
            if (playerList == null) return null;

            return new PlayerListResponse
            {
                Id = playerList.Id,
                Name = playerList.Name,
                PidId = playerList.PidId,
                CreatedDate = playerList.CreatedDate,
                UpdatedDate = playerList.UpdatedDate,
                PlayerIds = playerList.PlayerIds,
                PlayerCount = playerList.PlayerIds.Count
            };
        }

        public async Task<PlayerListResponse> CreateAsync(string pidId, CreatePlayerListRequest request)
        {
            var playerList = new PlayerList
            {
                Name = request.Name,
                PidId = pidId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                PlayerIds = []
            };

            var createdList = await playerListRepository.CreateAsync(playerList);

            return new PlayerListResponse
            {
                Id = createdList.Id,
                Name = createdList.Name,
                PidId = createdList.PidId,
                CreatedDate = createdList.CreatedDate,
                UpdatedDate = createdList.UpdatedDate,
                PlayerIds = createdList.PlayerIds,
                PlayerCount = createdList.PlayerIds.Count
            };
        }

        public async Task<PlayerListResponse> UpdateAsync(string id, UpdatePlayerListRequest request)
        {
            var existingList = await playerListRepository.GetByIdAsync(id);
            if (existingList == null) return null;

            existingList.Name = request.Name;
            existingList.UpdatedDate = DateTime.Now;

            var updatedList = await playerListRepository.UpdateAsync(existingList);

            return new PlayerListResponse
            {
                Id = updatedList.Id,
                Name = updatedList.Name,
                PidId = updatedList.PidId,
                CreatedDate = updatedList.CreatedDate,
                UpdatedDate = updatedList.UpdatedDate,
                PlayerIds = updatedList.PlayerIds,
                PlayerCount = updatedList.PlayerIds.Count
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await playerListRepository.DeleteAsync(id);
        }

        public async Task<bool> AddPlayerToListAsync(string listId, AddPlayerToListRequest request)
        {
            return await playerListRepository.AddPlayerToListAsync(listId, request.PlayerId);
        }

        public async Task<bool> RemovePlayerFromListAsync(string listId, string playerId)
        {
            return await playerListRepository.RemovePlayerFromListAsync(listId, playerId);
        }
    }
}
