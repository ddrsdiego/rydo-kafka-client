namespace Rydo.Kafka.Client.Logging
{
    internal static class KafkaClientLogField
    {
        public const string LogType = "{log}";
        public const string TopicName = "{topic}";
        public const string HandlerId = "{handlerid}";
        public const string ConsumerConfig = "{@ConsumerConfig}";
        public const string ConsumerHandlerName = "{ConsumerHandlerName}";
        public const string ElapsedMilliseconds = "{ElapsedMilliseconds}";
        
        public const string MessageKey = "{MessageKey}";
        public const string MessageValue = "{MessageValue}";
        
        public const string KafkaConsumerInLog = "{@KafkaConsumerInLog}";
        public const string KafkaProducerOutLog = "{@KafkaProducerOutLog}";
        public const string KafkaConsumerCommitLog = "{@KafkaConsumerCommitLog}";
        
        public const string ConfluentKafkaError = "{@ConfluentKafkaError}";
    }
}