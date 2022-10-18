using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Common.Interfaces.Services;

namespace Magnus.Futbot.Api.Workers
{
    public class DeactivateAllActionsOnStartUp : BackgroundService
    {
        private readonly IActionsService _actionsService;

        public DeactivateAllActionsOnStartUp(IActionsService actionsService)
        {
            _actionsService = actionsService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => Task.Run(() => _actionsService.DeactivateAllActionsOnStartUp(), stoppingToken);
    }
}
