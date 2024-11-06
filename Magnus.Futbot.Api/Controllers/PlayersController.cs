using Magnus.Futbot.Api.Caches;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Magnus.Futbot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController(CardsCache cardsCache) : Controller
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
        public IActionResult GetByAssetIds(string assetIds)
        {
            if (string.IsNullOrEmpty(assetIds))
                return BadRequest("AssetIds cannot be null or empty.");

            var assetIdList = assetIds.Split(',').Select(int.Parse).ToList();

            var players = cardsCache
                .Cards
                .Where(c => assetIdList.Contains(c.EAId))
                .ToList();

            return Ok(players);
        }
    }
}
