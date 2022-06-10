namespace Rydo.Kafka.Client.Serializations
{
    using System;
    using System.Text.Json;
    using Confluent.Kafka;

    internal sealed class RydoKafkaClientJsonDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<T>(data, SystemTextJsonMessageSerializer.Options);
        }
    }
}