using Magnus.Futbot.Selenium;

namespace Magnus.Futbot.Api.Services
{
    public class ActionsDeactivator
    {
        private readonly ProfilesService _profilesService;
        private readonly UserActionsService _userActionsService;

        public ActionsDeactivator(ProfilesService profilesService,
            UserActionsService userActionsService)
        {
            _profilesService = profilesService;
            _userActionsService = userActionsService;
        }

        public async Task DeactivateAction(string profileId, string actionId)
        {
            var profileDTO = await _profilesService.GetById(profileId);
            var action = _userActionsService.GetActionsQueueByUsername(profileDTO.Email).PendingActions.UnorderedItems.Select(i => i.Element).FirstOrDefault(a => a.Id == actionId);
            action?.CancellationTokenSource.Cancel();
        }

        public async Task DeactivateAllAction(string profileId)
        {
            var profileDTO = await _profilesService.GetById(profileId);
            var actions = _userActionsService.GetActionsQueueByUsername(profileDTO.Email).PendingActions.UnorderedItems.Select(i => i.Element);
            if (actions is null)
                return;

            foreach (var action in actions)
                action.CancellationTokenSource.Cancel();
        }
    }
}
