using Confluent.Kafka;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Kafka;

namespace Magnus.Futbot.Api.Kafka.Consumers
{
    public class ProfilesConsumer : BaseConsumer<Ignore, ProfileDTO>
    {
        public ProfilesConsumer(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Topic => $"Magnus.Futbot.Profiles";

        public override string GroupId => "Magnus.Futbot.Profiles.Consumer";
    }
}
