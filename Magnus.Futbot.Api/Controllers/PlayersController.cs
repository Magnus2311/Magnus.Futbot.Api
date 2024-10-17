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
        public IActionResult Get(int page = 1, int pageSize = 50)
        {
            var totalCount = cardsCache.Cards.Count; 
            
            var pagedPlayers = cardsCache
                .Cards
                .OrderByDescending(c => c.OverallRating)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                Data = pagedPlayers,
                TotalCount = totalCount,
                PageSize = pageSize
            };

            return Ok(response);
        }
    }
}
