using Confluent.Kafka;
using Magnus.Futbot.Common.Models.Kafka;

namespace Magnus.Futbot.Api.Kafka.Producers.Requests
{
    public class ProfilesRequest : BaseProducer<Null, string>
    {
        public ProfilesRequest(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Topic => "Magnus.Futbot.Profiles.Requests";
    }
}
