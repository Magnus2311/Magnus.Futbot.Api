using Confluent.Kafka;
using Magnus.Futbot.Common.Models.Kafka;
using Magnus.Futbot.Common.Models.Selenium.Profiles;

namespace Magnus.Futbot.Api.Kafka.Producers
{
    public class ProfileProducer : BaseProducer<Null, AddProfileDTO>
    {
        public ProfileProducer(IConfiguration configuration) : base(configuration)
        {

        }

        public override string Topic => "Magnus.Futbot.Profiles.Add";
    }
}
