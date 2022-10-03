using AutoMapper;
using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Services;

namespace Magnus.Futbot.Api.Services
{
    public class ActionsService
    {
        private readonly ProfilesService _profilesService;
        private readonly IMapper _mapper;

        public ActionsService(ProfilesService profilesService,
            IMapper mapper)
        {
            _profilesService = profilesService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TradeActionDTO>> GetPendingActionsByProfileId(string profileId)
        {
            var profileDTO = await _profilesService.GetById(profileId);
            var actions = _mapper.Map<IEnumerable<TradeActionDTO>>(DataSeleniumService.GetTradeActionsByProfile(profileDTO));
            return actions;
        }

        public async Task CancelActionById(string profileId, string actionId)
        {
            var profileDTO = await _profilesService.GetById(profileId);
            var actions = DataSeleniumService.GetTradeActionsByProfile(profileDTO);
            var actionToCancel = actions.FirstOrDefault(a => a.Id == actionId);
            actionToCancel?.CancellationTokenSource.Cancel();
        }
    }
}
