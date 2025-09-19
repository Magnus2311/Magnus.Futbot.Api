using Microsoft.AspNetCore.Mvc;
using Magnus.Futbot.Api.Helpers;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Api.Models.Requests;
using AutoMapper;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Common;
using Magnus.Futbot.Api.Caches;

namespace Magnus.Futbot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly TradeHistoryService _tradeHistoryService;
        private readonly CardsCache _cardsCache;

        public TradesController(TradeHistoryService tradeHistoryService, IMapper mapper, CardsCache cardsCache)
        {
            _tradeHistoryService = tradeHistoryService;
            _mapper = mapper;
            _cardsCache = cardsCache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string profileId)
        {
            return Ok(await _tradeHistoryService.GetAllTradesAsync(profileId));
        }

        [HttpGet("{pidId}")]
        public async Task<IActionResult> GetByPidIdAsync(string pidId)
        {
            return Ok(await _tradeHistoryService.GetAllTradesByPidIdAsync(pidId));
        }

        [HttpPost("buy/{pidId}")]
        public async Task<IActionResult> AddBuyAsync(string pidId, AddBuyTradeRequest addBuyTradeRequest)
        {
            var card = _cardsCache.Cards.FirstOrDefault(c => c.CardId == addBuyTradeRequest.CardId);
            var buyCardDTO = _mapper.Map<BuyCardDTO>(addBuyTradeRequest);
            if (card is not null)
                buyCardDTO.Card = card;

            var trade = new Trade
            {
                BuyCardDTO = buyCardDTO,
                PidId = pidId,
                TradeHistoryActionType = TradeHistoryActionType.Buy
            };

            await _tradeHistoryService.AddTradeAsync(pidId, buyCardDTO);

            return Ok();
        }
    }
}
