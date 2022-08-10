using Confluent.Kafka;
using Newtonsoft.Json;
using System.Text;

namespace Magnus.Futbot.Common.Models.Kafka.Serialization
{
    public class Deserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            => JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data))!;
    }
}
