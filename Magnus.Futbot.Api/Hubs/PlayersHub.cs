using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Hubs
{
    public class PlayersHub : Hub<IPlayersClient>
    {
        private readonly PlayersCache _playersCache;

        public PlayersHub(PlayersCache playersCache)
        {
            _playersCache = playersCache;
        }

        public async Task GetPlayers()
            => await Clients.All.OnPlayersLoaded(_playersCache.Players.Select(p => p.Value));
    }
}