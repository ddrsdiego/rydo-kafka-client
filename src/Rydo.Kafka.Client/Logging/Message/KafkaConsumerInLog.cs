namespace Rydo.Kafka.Client.Logging.Message
{
    using System;
    using Consumers;

    internal sealed class KafkaConsumerInLog : KafkaConsumerLog
    {
        public KafkaConsumerInLog(string batchId, string? consumer, string? cid, string? environment,
            ConsumerRecord consumerRecord) :
            base(KafkaClientLogType.MessageIn, consumer, batchId, cid, environment, consumerRecord)
        {
            MessageInAt = DateTime.UtcNow;
        }
        
        public DateTime MessageInAt { get; }
    }
}