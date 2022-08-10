using Confluent.Kafka;
using Magnus.Futbot.Common.Models.Kafka.Serialization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Magnus.Futbot.Common.Models.Kafka
{

    public abstract class BaseConsumer<TKey, TValue>
    {
        public BaseConsumer(IConfiguration configuration)
        {
            Configuration = configuration;

            var config = new ConsumerConfig
            {
                BootstrapServers = Configuration["URLS:KAFKA:SERVER"],
                GroupId = GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            Consumer = new ConsumerBuilder<TKey, TValue>(config)
                .SetValueDeserializer(new Deserializer<TValue>())
                .Build();
        }

        public IConsumer<TKey, TValue> Consumer { get; }
        public bool Cancelled { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public abstract string Topic { get; }
        public abstract string GroupId { get; }

        protected IConfiguration Configuration { get; }

        public void Subscribe()
        {
            Consumer.Subscribe(Topic);
        }

        public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default)
        {
            Consumer.Subscribe(Topic);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    return Consumer.Consume(cancellationToken);
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(ex));
                }
            }

            return new ConsumeResult<TKey, TValue>();
        }
    }
}
