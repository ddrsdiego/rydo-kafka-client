namespace Rydo.Kafka.Client.Logging.Message
{
    using Consumers;

    internal abstract class KafkaConsumerLog : KafkaMessageLog
    {
        protected KafkaConsumerLog(string log, string? consumer, string batchId, string? cid,
            string? environment,
            ConsumerRecord consumerRecord)
            : base(log, cid, environment, consumerRecord.Key(), consumerRecord.TopicName, consumerRecord.Partition,
                consumerRecord.Offset)
        {
            BatchId = batchId;
            Consumer = consumer;
        }

        public string BatchId { get; }
        public string? Consumer { get; }
    }
}