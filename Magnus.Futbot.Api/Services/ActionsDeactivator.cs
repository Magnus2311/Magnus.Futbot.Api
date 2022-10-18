using Magnus.Futbot.Services;

namespace Magnus.Futbot.Api.Services
{
    public class ActionsDeactivator
    {
        private readonly DataSeleniumService _dataSeleniumService;
        private readonly ProfilesService _profilesService;

        public ActionsDeactivator(
            DataSeleniumService dataSeleniumService,
            ProfilesService profilesService)
        {
            _dataSeleniumService = dataSeleniumService;
            _profilesService = profilesService;
        }

        public async Task DeactivateAction(string profileId, string actionId)
        {
            var profileDTO = await _profilesService.GetById(profileId);
            var action = _dataSeleniumService.GetTradeActionsByProfile(profileDTO).FirstOrDefault(a => a.Id == actionId);
            action?.CancellationTokenSource.Cancel();
        }
    }
}
