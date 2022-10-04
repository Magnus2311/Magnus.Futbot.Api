using AutoMapper;
using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Common.Interfaces.Notifiers;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Services.Notifiers
{
    public class ActionsNotifier : IActionsNotifier
    {
        private readonly IHubContext<ActionsHub, IActionsClient> _actionsContext;
        private readonly IMapper _mapper;

        public ActionsNotifier(
            IHubContext<ActionsHub, IActionsClient> actionsContext,
            IMapper mapper)
        {
            _actionsContext = actionsContext;
            _mapper = mapper;
        }

        public Task AddAction(ProfileDTO profileDTO, TradeAction tradeAction)
            => _actionsContext
                .Clients
                .Users(profileDTO.UserId)
                .OnActionAdded(_mapper.Map<TradeActionDTO>(tradeAction));
    }
}
