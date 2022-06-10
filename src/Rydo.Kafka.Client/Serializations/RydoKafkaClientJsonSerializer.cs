namespace Rydo.Kafka.Client.Serializations
{
    using System.Text.Json;
    using Confluent.Kafka;

    internal sealed class RydoKafkaClientJsonSerializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context) =>
            JsonSerializer.SerializeToUtf8Bytes(data, SystemTextJsonMessageSerializer.Options);
    }
}