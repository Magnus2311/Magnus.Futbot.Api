using Magnus.Futbot.Api.Helpers.Attributes;
using Magnus.Futbot.Api.Models.DTOs;
using Magnus.Futbot.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Magnus.Futbot.Api.Controllers
{
    [SSO]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly ProfilesService _profilesService;

        public ProfilesController(ProfilesService profilesService)
        {
            _profilesService = profilesService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
            => Ok(await _profilesService.GetAll());

        [HttpPost("add-profile")]
        public async Task<IActionResult> AddProfile(ProfileDTO profileDTO)
            => Ok(await _profilesService.Add(profileDTO));

        [HttpPost("submit-code")]
        public async Task<IActionResult> SubmitCode(SubmitCodeDTO submitCodeDTO)
            => Ok(await _profilesService.SubmitCode(submitCodeDTO));
    }
}