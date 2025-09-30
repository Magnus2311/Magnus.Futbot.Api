using AutoMapper;
using Magnus.Futbot.Api.Models.Requests;
using Magnus.Futbot.Api.Models.Responses;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace Magnus.Futbot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricesController(PriceService priceService, IMapper mapper) : ControllerBase
    {
        [HttpGet("{cardId}")]
        public async Task<IActionResult> Get(string cardId)
        {
            var playerPrice = await priceService.Get(cardId);

            var getPriceResponse = mapper.Map<GetPriceResponse>(playerPrice);

            return Ok(getPriceResponse);
        }

        [HttpPost]
        public async Task<IActionResult> SavePrices(AddPlayerPricesRequest addPlayerPricesRequest)
        {
            var playerPrice = mapper.Map<PlayerPrice>(addPlayerPricesRequest);

            await priceService.Add(playerPrice);

            return Ok(new { success = true, message = "Prices saved successfully" });
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> GetBulkPrices(BulkPricesRequest bulkPricesRequest)
        {
            if (bulkPricesRequest.CardIds == null || !bulkPricesRequest.CardIds.Any())
            {
                return BadRequest("CardIds cannot be null or empty.");
            }

            var playerPrices = await priceService.Get(bulkPricesRequest.CardIds);

            var result = playerPrices.ToDictionary(
                pp => pp.CardId,
                pp => new
                {
                    prices = pp.Prices.Select(p => p.Prize).ToList(),
                    lastUpdated = pp.Prices.Any() ? pp.Prices.Max(p => p.UpdatedDate) : DateTime.MinValue
                }
            );

            return Ok(result);
        }
    }
}
