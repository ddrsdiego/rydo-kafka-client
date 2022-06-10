namespace Rydo.Kafka.Client.Logging.Message
{
    using System;
    using Consumers;

    internal sealed class KafkaConsumerCommitLog : KafkaConsumerLog
    {
        public KafkaConsumerCommitLog(string batchId, string? consumer, string? cid, string? environment,
            ConsumerRecord consumerRecord) :
            base(KafkaClientLogType.CommitRecord, consumer, batchId, cid, environment, consumerRecord)
        {
            CommittedAt = DateTime.UtcNow;
        }

        public DateTime CommittedAt { get; }
    }
}