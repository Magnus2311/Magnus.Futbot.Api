using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Common.Models.DTOs.Profiles;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Hubs
{
    public class ProfilesHub : Hub<IProfilesClient>
    {
        private readonly ProfilesService _profilesService;

        public ProfilesHub(ProfilesService profilesService)
        {
            _profilesService = profilesService;
        }

        public async Task AddProfile(AddProfileDTO profileDTO)
        {
            var userId = Context.UserIdentifier ?? "";
            profileDTO.UserId = userId;
            var loginResponse = await _profilesService.Add(profileDTO);
            await Clients.Users(userId).OnProfileAdded(loginResponse);
        }

        public async Task GetProfiles()
        {
            var userId = Context.UserIdentifier ?? "";
            var profiles = await _profilesService.GetAll(userId);
            await Clients.Users(userId).OnProfilesLoaded(profiles);
        }

        public async Task SubmitCode(SubmitCodeDTO submitCodeDTO)
        {
            var userId = Context.UserIdentifier ?? "";
            submitCodeDTO.UserId = userId;
            var response = await _profilesService.SubmitCode(submitCodeDTO);
            await Clients.Users(userId).OnCodeSubmited(response);
        }

        public async Task OnProfileRefresh(string profileId)
        {
            var userId = Context.UserIdentifier ?? "";
            var profileDTO = await _profilesService.RefreshProfile(profileId, userId);
            await Clients.Users(userId).OnProfileUpdated(profileDTO);
        }

        public async Task EditProfile(EditProfileDTO editProfileDTO)
            => await Clients.Users(Context.UserIdentifier ?? "").OnProfileUpdated(await _profilesService.EditProfile(editProfileDTO));
    }
}