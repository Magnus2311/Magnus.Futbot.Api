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
    }
}
