using Confluent.Kafka;
using Magnus.Futbot.Common.Models.Kafka.Serialization;
using Microsoft.Extensions.Configuration;

namespace Magnus.Futbot.Common.Models.Kafka
{
    public class ResponseConsumer<TKey, TValue>
    {
        private readonly IConfiguration _configuration;

        public ResponseConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private long LastOffset { get; set; }

        public IEnumerable<TValue> FetchData(string topic, string groupId, TKey key)
        {
            var data = new List<TValue>();
            var consumer = GetConsumer(groupId);
            consumer.Subscribe(topic);
            while (true)
            {
                var message = consumer.Consume();
                if (message is not null && EqualityComparer<TKey>.Default.Equals(message.Message.Key, key))
                {
                    data.Add(message.Message.Value);

                    if (message.Offset == LastOffset) break;
                }
            }

            return data;
        }

        public IEnumerable<TValue> FetchData(string topic, string groupId)
        {
            var data = new List<TValue>();

            var consumer = GetConsumer(groupId);
            consumer.Subscribe(topic);
            while (true)
            {
                var message = consumer.Consume();
                if (message is not null)
                {
                    data.Add(message.Message.Value);

                    if (message.Offset == LastOffset) break;
                }
            }

            return data;
        }

        public IConsumer<TKey, TValue> GetConsumer(string groupId)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["URLS:KAFKA:SERVER"],
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            return new ConsumerBuilder<TKey, TValue>(config)
                .SetPartitionsAssignedHandler((c, tps) =>
                {
                    var partitionOffsets = c.Committed(tps, TimeSpan.FromMilliseconds(10));
                    var watermarkOffsets = tps.Select(tp => c.QueryWatermarkOffsets(tp, TimeSpan.FromSeconds(10)));
                    var offsets = watermarkOffsets.Zip(partitionOffsets, (watermarkOffset, topicPartitionOffset) =>
                    {
                        LastOffset = watermarkOffset.High;
                        if (topicPartitionOffset.Offset.IsSpecial || watermarkOffset.High.IsSpecial)
                        {
                            return topicPartitionOffset.Offset;
                        }

                        var lastTopicOffset = watermarkOffset.High - 1;
                        var lastCommittedOffset = topicPartitionOffset.Offset - 1;

                        if (lastTopicOffset == 0)
                        {
                            return new Offset(0);
                        }
                        if (lastCommittedOffset == lastTopicOffset)
                        {
                            return new Offset(lastCommittedOffset);
                        }
                        return new Offset(lastCommittedOffset + 1);
                    });

                    return tps.Zip(offsets, (partition, offset) => new TopicPartitionOffset(partition, offset));
                })
                .SetValueDeserializer(new Deserializer<TValue>())
                .Build();
        }
    }
}
