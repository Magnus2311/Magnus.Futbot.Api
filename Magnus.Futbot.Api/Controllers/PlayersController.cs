using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Magnus.Futbot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController(CardsCache cardsCache, PriceService priceService) : Controller
    {
        [HttpGet]
        public IActionResult Get(int page = 1, int pageSize = 50, string search = "")
        {
            var totalFilteredCards = cardsCache
                .Cards
                .Where(c => c.Name.ToLower().Contains(search.ToLower()))
                .OrderByDescending(c => c.OverallRating)
                .DistinctBy(c => new { c.OverallRating, c.Name, c.EAId }); 
            
            var pagedPlayers = totalFilteredCards
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                Data = pagedPlayers,
                TotalCount = totalFilteredCards.Count(),
                PageSize = pageSize
            };

            return Ok(response);
        }


        [HttpGet("byAssetIds")]
        public async Task<IActionResult> GetByAssetIds(string assetIds)
        {
            if (string.IsNullOrEmpty(assetIds))
                return BadRequest("AssetIds cannot be null or empty.");

            var assetIdList = assetIds.Split(',').Select(int.Parse).ToList();

            var players = cardsCache
                .Cards
                .Where(c => assetIdList.Contains(c.EAId))
                .ToList();

            var cardIds = players.Select(p => p.CardId.ToString()).ToList();
            var prices = await priceService.Get(cardIds);

            var playersWithPrices = players.Select(player => new
            {
                Player = player,
                Prices = prices.Where(p => p.CardId == player.CardId).Select(pp => pp.Prices.Select(p => p.Prize))
            }).ToList();

            return Ok(playersWithPrices);
        }
    }
}
