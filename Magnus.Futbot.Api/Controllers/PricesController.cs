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
    }
}
