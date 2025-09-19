using AutoMapper;
using Magnus.Futbot.Common.Interfaces.Helpers;
using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Database.Repositories;
using Magnus.Futbot.Initializer.Connections;
using Magnus.Futbot.Initializer.Models.Players;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Magnus.Futbot.Initializer
{
    public class RefreshPlayersWorker : BackgroundService
    {
        private readonly EaConnectionService _eaConnectionService;
        private readonly ILogger<RefreshPlayersWorker> _logger;
        private readonly IPlayersService _playersService;
        private readonly IMapper _mapper;

        public RefreshPlayersWorker(ILogger<RefreshPlayersWorker> logger,
            EaConnectionService eaConnectionService,
            IPlayersService playersService,
            IMapper mapper)
        {
            _eaConnectionService = eaConnectionService;
            _playersService = playersService;
            _logger = logger;
            _mapper = mapper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var players = await FetchPlayers();
                    await _playersService.Add(players);

                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(JsonConvert.SerializeObject(ex));
                    await Task.Delay(TimeSpan.FromDays(10), stoppingToken);
                }
            }
        }

        private async Task<IEnumerable<PlayerDTO>> FetchPlayers()
        {
            try
            {
                var allItems = await _eaConnectionService.FetchAllPlayers();
                return _mapper.Map<IEnumerable<PlayerDTO>>(allItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(JsonConvert.SerializeObject(ex));
                return Enumerable.Empty<PlayerDTO>();
            }
        }
    }
}