using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Database.Models;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Services.Connections.SignalR
{
    public class PlayersConnection
    {
        private readonly IHubContext<PlayersHub, IPlayersClient> _playersHub;

        public PlayersConnection(IHubContext<PlayersHub, IPlayersClient> playersHub)
        {
            _playersHub = playersHub;
        }

        public async Task AddPlayer(PlayerDocument player)
            => await _playersHub.Clients.All.OnPlayerAdded(player);
    }
}