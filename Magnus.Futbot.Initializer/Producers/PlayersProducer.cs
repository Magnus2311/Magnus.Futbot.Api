using Confluent.Kafka;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Kafka;

namespace Magnus.Futbot.Initializer.Producers
{
    public class PlayersProducer : BaseProducer<Null, PlayerDTO>
    {
        public PlayersProducer(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Topic => "EA.Players.All";
    }
}
