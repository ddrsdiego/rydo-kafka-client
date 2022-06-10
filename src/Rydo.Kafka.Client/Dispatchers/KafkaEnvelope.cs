namespace Rydo.Kafka.Client.Dispatchers
{
    using System;
    using System.Text.Json.Serialization;

    internal sealed class KafkaEnvelope
    {
        [JsonConstructor]
        public KafkaEnvelope(string topic, string key, byte[] payload, byte[] meta)
        {
            Topic = topic;
            Key = key;
            Payload = payload;
            Meta = meta;
            CreatedAt = DateTime.Now;
        }

        public string Topic { get; }
        public string Key { get; }
        public byte[] Payload { get; }
        public byte[] Meta { get; }
        public DateTime CreatedAt { get; }

        public static KafkaEnvelope CreateInstance(string topic, string key, byte[] payload, byte[] meta) =>
            new KafkaEnvelope(topic, key, payload, meta);
    }
}