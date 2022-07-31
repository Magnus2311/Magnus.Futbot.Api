using Confluent.Kafka;
using Magnus.Futbot.Api.Kafka.Consumers;
using Magnus.Futbot.Api.Kafka.Producers.Requests;
using Magnus.Futbot.Common.Models.DTOs;

namespace Magnus.Futbot.Api.Kafka.Fetchers
{
    public class ProfilesFetcher
    {
        private readonly IConfiguration _configuration;
        private readonly ProfilesRequest _profilesRequest;

        public ProfilesFetcher(IConfiguration configuration,
            ProfilesRequest profilesRequest)
        {
            _configuration = configuration;
            _profilesRequest = profilesRequest;
        }

        public IEnumerable<ProfileDTO> FetchProfiles(string userId)
        {
            _profilesRequest.Produce(userId);

            var profiles = new Dictionary<string, ProfileDTO>();

            var profilesConsumer = new UserProfilesConsumer(_configuration, userId);
            profilesConsumer.Subscribe();

            while (true)
            {
                var message = profilesConsumer.Consumer.Consume(TimeSpan.FromMilliseconds(150));
                if (message is null) break;

                if (profiles.ContainsKey(message.Message.Value.Email))
                    profiles[message.Message.Value.Email] = message.Message.Value;
                else
                    profiles.Add(message.Message.Value.Email, message.Message.Value);

            }

            return profiles.Values;
        }
    }
}
