using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Models.DTOs;
using Magnus.Futbot.Api.Services;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Hubs
{
    public class ProfilesHub : Hub<IProfilesClient>
    {
        private readonly ProfilesService _profilesService;

        public ProfilesHub(ProfilesService profilesService)
        {
            _profilesService = profilesService;
        }

        public async Task AddProfile(ProfileDTO profileDTO)
        {
            var userId = new ObjectId(Context.UserIdentifier ?? "");
            profileDTO.UserId = userId;
            var loginResponse = await _profilesService.Add(profileDTO);
            await Clients.Users(userId.ToString()).OnProfileAdded(loginResponse);
        }

        public async Task GetProfiles()
        {
            var userId = new ObjectId(Context.UserIdentifier ?? "");
            var profiles = await _profilesService.GetAll(userId);
            await Clients.User(userId.ToString()).OnProfilesLoaded(profiles);
        }

        public async Task SubmitCode(SubmitCodeDTO submitCodeDTO)
        {
            var userId = new ObjectId(Context.UserIdentifier ?? "");
            submitCodeDTO.UserId = userId;
            var response = await _profilesService.SubmitCode(submitCodeDTO);
            await Clients.Users(userId.ToString()).OnCodeSubmited(response);
        }
    }
}