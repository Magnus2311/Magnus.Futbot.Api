using Magnus.Futbot.Api.Services;

namespace Magnus.Futbot.Api.Workers
{
    public class DeactivateAllActionsOnStartUp : BackgroundService
    {
        private readonly ActionsService _actionsService;

        public DeactivateAllActionsOnStartUp(ActionsService actionsService)
        {
            _actionsService = actionsService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => Task.Run(() =>
            {
                _actionsService.DeactivateAllActionsOnStartUp();
            }, stoppingToken);
    }
}
