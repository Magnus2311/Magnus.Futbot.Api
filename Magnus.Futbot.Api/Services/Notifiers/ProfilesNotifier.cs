using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Common.Models.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Services.Notifiers
{
    public class ProfilesNotifier
    {
        private readonly IHubContext<ProfilesHub, IProfilesClient> _profilesContext;

        public ProfilesNotifier(IHubContext<ProfilesHub, IProfilesClient> profilesContext)
        {
            _profilesContext = profilesContext;
        }

        public Task RefreshProfile(ProfileDTO profileDTO)
            => _profilesContext
                .Clients
                .Users(profileDTO.UserId)
                .OnProfileUpdated(profileDTO);
    }
}
