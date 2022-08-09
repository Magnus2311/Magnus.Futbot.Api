using Confluent.Kafka;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Kafka;

namespace Magnus.Futbot.Api.Kafka.Consumers
{

    public class UserProfilesConsumer : ResponseConsumer<Ignore, ProfileDTO>
    {
        public UserProfilesConsumer(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
