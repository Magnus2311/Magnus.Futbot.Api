using Microsoft.AspNetCore.Mvc;
using Magnus.Futbot.Api.Helpers;

namespace Magnus.Futbot.Api.Controllers
{
    public class TradesController : Controller
    {
        [HttpGet]
        [SSOVerification]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }
    }
}
