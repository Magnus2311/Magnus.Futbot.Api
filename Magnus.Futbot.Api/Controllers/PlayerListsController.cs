using Microsoft.AspNetCore.Mvc;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Api.Models.Requests;

namespace Magnus.Futbot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerListsController : Controller
    {
        private readonly PlayerListService _playerListService;

        public PlayerListsController(PlayerListService playerListService)
        {
            _playerListService = playerListService;
        }

        [HttpGet("{pidId}")]
        public async Task<IActionResult> GetByPidIdAsync(string pidId)
        {
            var playerLists = await _playerListService.GetByPidIdAsync(pidId);
            return Ok(playerLists);
        }

        [HttpGet("list/{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var playerList = await _playerListService.GetByIdAsync(id);
            if (playerList == null)
                return NotFound();

            return Ok(playerList);
        }

        [HttpPost("{pidId}")]
        public async Task<IActionResult> CreateAsync(string pidId, CreatePlayerListRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var playerList = await _playerListService.CreateAsync(pidId, request);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = playerList.Id }, playerList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, UpdatePlayerListRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var playerList = await _playerListService.UpdateAsync(id, request);
            if (playerList == null)
                return NotFound();

            return Ok(playerList);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var result = await _playerListService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("{id}/players")]
        public async Task<IActionResult> AddPlayerToListAsync(string id, AddPlayerToListRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _playerListService.AddPlayerToListAsync(id, request);
            if (!result)
                return NotFound();

            return Ok();
        }

        [HttpDelete("{id}/players/{playerId}")]
        public async Task<IActionResult> RemovePlayerFromListAsync(string id, string playerId)
        {
            var result = await _playerListService.RemovePlayerFromListAsync(id, playerId);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
