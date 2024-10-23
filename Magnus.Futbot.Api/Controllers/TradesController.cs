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
        [SSOVerification]
        public async Task<IActionResult> GetAsync(string profileId)
        {
            var a = profileId;
            var userId = Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null)
                return Unauthorized();

            return Ok(await _tradeHistoryService.GetAllTradesAsync(profileId, userId));
        }

        [HttpGet("pidid/{pidId}")]
        [SSOVerification]
        public async Task<IActionResult> GetByPidIdAsync(string pidId)
        {
            var userId = Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null)
                return Unauthorized();

            return Ok(await _tradeHistoryService.GetAllTradesAsync(pidId, userId));
        }

        [HttpPost("buy/{pidId}")]
        [SSOVerification]
        public async Task<IActionResult> AddBuyAsync(string pidId, AddBuyTradeRequest addBuyTradeRequest)
        {
            var userId = Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null)
                return Unauthorized();

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

            await _tradeHistoryService.AddTradeAsync(pidId, userId, buyCardDTO);

            return Ok();
        }
    }
}
