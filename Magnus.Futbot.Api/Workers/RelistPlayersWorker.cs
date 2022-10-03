using Magnus.Futbot.Api.Services.Interfaces;

namespace Magnus.Futbot.Api.Workers
{
    public class RelistPlayersWorker : BackgroundService
    {
        private readonly ITradingService _tradingService;

        public RelistPlayersWorker(ITradingService tradingService)
        {
            _tradingService = tradingService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await _tradingService.RelistPlayers();
                    await Task.Delay(TimeSpan.FromMinutes(65), stoppingToken);
                }
            }, stoppingToken);
        }
    }
}
