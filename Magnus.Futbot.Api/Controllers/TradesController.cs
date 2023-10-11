using Microsoft.AspNetCore.Mvc;
using Magnus.Futbot.Api.Helpers;
using Magnus.Futbot.Api.Services;

namespace Magnus.Futbot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradesController : Controller
    {
        private readonly TradeHistoryService _tradeHistoryService;

        public TradesController(TradeHistoryService tradeHistoryService)
        {
            _tradeHistoryService = tradeHistoryService;
        }

        [HttpGet]
        [SSOVerification]
        public async Task<IActionResult> Get(string profileId)
        {
            var a = profileId;
            var userId = Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null)
                return Unauthorized();

            return Ok(await _tradeHistoryService.GetAllTrades(profileId, userId));
        }
    }
}
