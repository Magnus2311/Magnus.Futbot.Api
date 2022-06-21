using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Models.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Services.Connections.SignalR
{
    public class ProfilesConnection
    {
        private readonly IHubContext<ProfilesHub, IProfilesClient> _profilesHub;

        public ProfilesConnection(IHubContext<ProfilesHub, IProfilesClient> profilesHub)
        {
            _profilesHub = profilesHub;
        }

        public async Task UpdateProfile(ProfileDTO profile)
            => await _profilesHub.Clients.Users(profile.UserId.ToString()).OnProfileUpdated(profile);
    }
}