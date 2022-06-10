namespace Rydo.Kafka.Client.Logging.Message
{
    using System;
    using Confluent.Kafka;

    internal sealed class KafkaConsumerOutLog : KafkaProducerLog
    {
        public KafkaConsumerOutLog(string? producer, string? cid, string? environment,
            DeliveryResult<byte[], byte[]>? deliveryResult)
            : base(KafkaClientLogType.MessageOut, cid, environment, deliveryResult)
        {
            Producer = producer;
            MessageOutAt = DateTime.UtcNow;
        }

        public string? Producer { get; }
        public DateTime MessageOutAt { get; }
    }
}