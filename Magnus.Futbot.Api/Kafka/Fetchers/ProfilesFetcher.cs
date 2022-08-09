using Magnus.Futbot.Api.Kafka.Consumers;
using Magnus.Futbot.Api.Kafka.Producers.Requests;
using Magnus.Futbot.Common.Models.DTOs;

namespace Magnus.Futbot.Api.Kafka.Fetchers
{
    public class ProfilesFetcher
    {
        private readonly ProfilesRequest _profilesRequest;
        private readonly UserProfilesConsumer _userProfilesConsumer;

        public ProfilesFetcher(ProfilesRequest profilesRequest,
            UserProfilesConsumer userProfilesConsumer)
        {
            _profilesRequest = profilesRequest;
            _userProfilesConsumer = userProfilesConsumer;
        }

        public Task RequestProfiles(string userId)
            => _profilesRequest.Produce(userId);
        

        public IEnumerable<ProfileDTO> FetchProfiles(string userId)        
            => _userProfilesConsumer.FetchData($"Magnus.Futbot.Profiles.{userId}", "Magnus.Futbot.Profiles.Fetcher");
    }
}
