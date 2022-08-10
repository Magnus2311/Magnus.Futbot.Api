using Confluent.Kafka;
using Newtonsoft.Json;
using System.Text;

namespace Magnus.Futbot.Common.Models.Kafka.Serialization
{
    public class Serializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
            => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
    }
}
