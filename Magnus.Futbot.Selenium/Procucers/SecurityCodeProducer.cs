using Confluent.Kafka;
using Magnus.Futbot.Common.Models.Kafka;
using Magnus.Futbot.Common.Models.Results;

namespace Magnus.Futbot.Selenium.Procucers
{
    public class SecurityCodeProducer : BaseProducer<Null, SubmitCodeResult>
    {
        public SecurityCodeProducer(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Topic => "Magnus.Futbot.Selenium.Submit.Code.Result";
    }
}
