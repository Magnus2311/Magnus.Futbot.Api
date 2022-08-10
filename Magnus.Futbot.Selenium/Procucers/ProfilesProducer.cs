using Confluent.Kafka;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Kafka;

namespace Magnus.Futbot.Selenium.Procucers
{
    public class ProfilesProducer : BaseProducer<Null, ProfileDTO>
    {
        public ProfilesProducer(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Topic => "Magnus.Futbot.Profiles";
    }
}