using AutoMapper;
using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Api.Models.EA;
using Magnus.Futbot.Api.Services.Connections;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;
using Newtonsoft.Json;

namespace Magnus.Futbot.Api.Services.Players
{
    public class InitPlayersService
    {
        private readonly EaConnectionService _eaConnectionService;
        private readonly PlayersRepository _playersRepository;
        private readonly PlayersCache _playersCache;
        private readonly IMapper _mapper;

        public InitPlayersService(EaConnectionService eaConnectionService,
            PlayersRepository playersRepository,
            PlayersCache playersCache,
            IMapper mapper)
        {
            _eaConnectionService = eaConnectionService;
            _playersRepository = playersRepository;
            _playersCache = playersCache;
            _mapper = mapper;
        }

        public async Task Init()
        {
            await FetchPlayers();
        }

        private async Task FetchPlayers()
        {
            var response = await _eaConnectionService.FetchPlayers();
            if (response is not null)
            {
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(response);
                var players = _mapper.Map<IEnumerable<PlayerDocument>>(myDeserializedClass.Players);
                var legendPlayers = _mapper.Map<IEnumerable<PlayerDocument>>(myDeserializedClass.LegendsPlayers);

                var allPlayers = players.Concat(legendPlayers);
                if (allPlayers is not null)
                {
                    var playersToInsert = allPlayers.Where(p => !_playersCache.Players.TryGetValue(p.Id, out _));
                    if (playersToInsert.Any())
                        await _playersRepository.AddPlayers(playersToInsert);
                }
            }
        }
    }
}