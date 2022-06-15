using Magnus.Futbot.Api.Models.DTOs;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Common;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Hubs
{
    public class ProfilesHub : Hub<IProfilesClient>
    {
        private readonly ProfilesService _profilesService;
        private readonly AppSettings _appSettings;

        public ProfilesHub(ProfilesService profilesService,
            AppSettings appSettings)
        {
            _profilesService = profilesService;
            _appSettings = appSettings;
        }

        public async Task AddProfile(ProfileDTO profileDTO)
        {
            var userId = Context.User;
            var loginResponse = new LoginResponseDTO(Helpers.LoginStatusType.ConfirmationKeyRequired, profileDTO);
            await Clients.All.OnProfileAdded(loginResponse);
        }
    }
}