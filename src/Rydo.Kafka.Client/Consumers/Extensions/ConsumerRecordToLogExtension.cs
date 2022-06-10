namespace Rydo.Kafka.Client.Consumers.Extensions
{
    using Constants;
    using Logging;
    using Logging.Message;
    using Microsoft.Extensions.Logging;

    internal static class ConsumerRecordToLogExtension
    {
        public static void LogMessageInComing(this ConsumerRecord consumerRecord, string batchId,
            ILogger? logger)
        {
            consumerRecord.Headers.TryGetValue(MessageHeadersDefault.Env, out var env);
            consumerRecord.Headers.TryGetValue(MessageHeadersDefault.Producer, out var producerName);
            consumerRecord.Headers.TryGetValue(MessageHeadersDefault.CorrelationId, out var cid);

            var log = new KafkaConsumerInLog(batchId, producerName?.ToString(), cid?.ToString(), env?.ToString(),
                consumerRecord);

            logger?.LogInformation($"[{KafkaClientLogField.LogType}] | {KafkaClientLogField.KafkaConsumerInLog}",
                log.Log,
                log);
        }

        public static void LogMessageCommit(this ConsumerRecord consumerRecord, string consumerRecordId,
            ILogger? logger)
        {
            consumerRecord.Headers.TryGetValue(MessageHeadersDefault.Env, out var env);
            consumerRecord.Headers.TryGetValue(MessageHeadersDefault.Producer, out var producerName);
            consumerRecord.Headers.TryGetValue(MessageHeadersDefault.CorrelationId, out var cid);

            var log = new KafkaConsumerCommitLog(consumerRecordId, producerName?.ToString(), cid?.ToString(),
                env?.ToString(), consumerRecord);
            
            logger?.LogInformation($"[{KafkaClientLogField.LogType}] | {KafkaClientLogField.KafkaConsumerCommitLog}",
                log.Log,
                log);
        }
    }
}