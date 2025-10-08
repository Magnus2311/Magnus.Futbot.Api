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
                .Where(c => assetIdList.Contains(c.EAId) || assetIdList.Contains(c.AssetId))
                .ToList();


            var cardIds = players.Select(p => p.EAId.ToString()).ToList();
            var prices = await priceService.Get(cardIds);

            var playersWithPrices = players.Select(player => new
            {
                player = new
                {
                    eaId = player.EAId,
                    cardId = player.CardId,
                    name = player.Name,
                    overallRating = player.OverallRating,
                    assetId = player.AssetId != 0 ? player.AssetId : player.EAId,
                    clubId = player.ClubId,
                    leagueId = player.LeagueId,
                    nationId = player.NationId,
                    shieldUrl = player.ShieldUrl
                },
                prices = prices.Where(p => p.CardId == player.CardId)
                    .SelectMany(pp => pp.Prices)
                    .Select(p => p.Prize)
                    .ToList()
            }).ToList();

            return Ok(playersWithPrices);
        }
    }
}
