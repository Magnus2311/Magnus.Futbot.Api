using Confluent.Kafka;
using Magnus.Futbot.Common.Models.Kafka;
using Magnus.Futbot.Common.Models.Selenium.Profiles;

namespace Magnus.Futbot.Selenium.Consumers
{
    public class InitProfileConsumer : BaseConsumer<Ignore, AddProfileDTO>
    {
        public InitProfileConsumer(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Topic => "Magnus.Futbot.Profiles.Add";

        public override string GroupId => "Selenium.Service.Profiles.Consumer";
    }
}