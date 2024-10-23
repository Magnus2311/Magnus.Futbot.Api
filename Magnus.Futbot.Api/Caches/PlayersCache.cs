using System.Collections.Concurrent;
using Magnus.Futbot.Api.Services.Connections.SignalR;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;
using MongoDB.Driver;

namespace Magnus.Futbot.Api.Caches
{
    public class PlayersCache
    {
        private readonly PlayersConnection _playersConnection;
        private readonly PlayersRepository _playersRepository;

        public PlayersCache(PlayersRepository playersRepository,
            PlayersConnection playersConnection)
        {
            _playersRepository = playersRepository;
            _playersConnection = playersConnection;

            Players = new ConcurrentDictionary<int, PlayerDocument>(_playersRepository.GetAll().Result.ToDictionary(p => p.Id, p => p));
            Task.Run(OnPlayerAdded);
        }

        public ConcurrentDictionary<int, PlayerDocument> Players { get; }

        private async Task OnPlayerAdded()
        {
            var enumerator = _playersRepository.Cursor.ToEnumerable().GetEnumerator();
            while (enumerator.MoveNext())
            {
                var player = enumerator.Current.FullDocument;
                Players.TryAdd(player.Id, player);
                await _playersConnection.AddPlayer(player);
            }
        }
    }
}