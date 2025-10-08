using Magnus.Futbot.Api.Models.Requests;
using Magnus.Futbot.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Magnus.Futbot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuccessfulPurchasesController(SuccessfulPurchaseService successfulPurchaseService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddSuccessfulPurchase(AddSuccessfulPurchaseRequest request)
        {
            try
            {
                var result = await successfulPurchaseService.AddSuccessfulPurchaseAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{pidId}")]
        public async Task<IActionResult> GetByPidId(string pidId)
        {
            try
            {
                var purchases = await successfulPurchaseService.GetByPidIdAsync(pidId);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{pidId}/count")]
        public async Task<IActionResult> GetCountByPidId(string pidId)
        {
            try
            {
                var count = await successfulPurchaseService.GetCountByPidIdAsync(pidId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{pidId}/date-range")]
        public async Task<IActionResult> GetByPidIdAndDateRange(string pidId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var purchases = await successfulPurchaseService.GetByPidIdAndDateRangeAsync(pidId, startDate, endDate);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{pidId}/filtered")]
        public async Task<IActionResult> GetFilteredPurchases(string pidId, [FromQuery] string position = null, [FromQuery] string quality = null, [FromQuery] string league = null, [FromQuery] string club = null)
        {
            try
            {
                var purchases = await successfulPurchaseService.GetFilteredPurchasesAsync(pidId, position, quality, league, club);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{pidId}/filtered/count")]
        public async Task<IActionResult> GetFilteredPurchasesCount(string pidId)
        {
            try
            {
                var count = await successfulPurchaseService.GetFilteredPurchasesCountAsync(pidId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
