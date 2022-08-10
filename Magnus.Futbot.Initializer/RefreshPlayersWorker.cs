using AutoMapper;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Initializer.Connections;
using Magnus.Futbot.Initializer.Models.Players;
using Magnus.Futbot.Initializer.Producers;
using Newtonsoft.Json;

namespace Magnus.Futbot.Initializer
{
    public class RefreshPlayersWorker : BackgroundService
    {
        private readonly ILogger<RefreshPlayersWorker> _logger;
        private readonly IMapper _mapper;
        private readonly EaConnectionService _eaConnectionService;
        private readonly PlayersProducer _playersProducer;

        public RefreshPlayersWorker(ILogger<RefreshPlayersWorker> logger,
            EaConnectionService eaConnectionService,
            PlayersProducer playersProducer,
            IMapper mapper)
        {
            _logger = logger;
            _eaConnectionService = eaConnectionService;
            _mapper = mapper;
            _playersProducer = playersProducer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var players = await FetchPlayers();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var player in players.Take(10))
                        await _playersProducer.Produce(player);

                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(JsonConvert.SerializeObject(ex));
                }
            }
            _playersProducer.Producer.Flush(stoppingToken);
        }

        private async Task<IEnumerable<PlayerDTO>> FetchPlayers()
        {
            var response = await _eaConnectionService.FetchPlayers();
            if (response is not null)
            {
                Root? myDeserializedClass = JsonConvert.DeserializeObject<Root>(response);
                if (myDeserializedClass is not null)
                {
                    var players = _mapper.Map<IEnumerable<PlayerDTO>>(myDeserializedClass.Players);
                    var legendPlayers = _mapper.Map<IEnumerable<PlayerDTO>>(myDeserializedClass.LegendsPlayers);

                    return players.Concat(legendPlayers);
                }
            }

            return Enumerable.Empty<PlayerDTO>();
        }
    }
}