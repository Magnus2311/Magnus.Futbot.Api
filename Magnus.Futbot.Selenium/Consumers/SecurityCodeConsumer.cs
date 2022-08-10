using Confluent.Kafka;
using Magnus.Futbot.Common.Models.Kafka;
using Magnus.Futbot.Common.Models.Selenium.Profiles;

namespace Magnus.Futbot.Selenium.Consumers
{
    public class SecurityCodeConsumer : BaseConsumer<Ignore, SubmitCodeDTO>
    {
        public SecurityCodeConsumer(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Topic => "Magnus.Futbot.Selenium.Submit.Code";

        public override string GroupId => "Selenium.Service.Security.Code.Consumer";
    }
}
