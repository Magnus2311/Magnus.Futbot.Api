using Confluent.Kafka;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Kafka;

namespace Magnus.Futbot.Api.Kafka.Consumers
{

    public class UserProfilesConsumer : BaseConsumer<Ignore, ProfileDTO>
    {
        private readonly string _userId;

        public UserProfilesConsumer(IConfiguration configuration,
            string userId) : base(configuration)
        {
            _userId = userId;
        }

        public override string Topic => $"Magnus.Futbot.Profiles.{_userId}";

        public override string GroupId => Guid.NewGuid().ToString();
    }
}
