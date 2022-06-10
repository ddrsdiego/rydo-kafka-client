namespace Rydo.Kafka.Client.Configurations
{
    public abstract class TopicDefinition
    {
        protected TopicDefinition(string topicName, string direction, string cryptKey, string consumerGroup,
            int retryAttempts = 0, string retryInterval = "")
        {
            TopicName = topicName.ToUpperInvariant();
            ConsumerGroup = consumerGroup.ToUpperInvariant();
            Direction = direction.ToLowerInvariant();
            CryptKey = cryptKey;
            RetryAttempts = retryAttempts;
            RetryInterval = retryInterval;
        }

        public string TopicName { get; }
        public string Direction { get; }
        public string CryptKey { get; }
        public int RetryAttempts { get; }
        public string RetryInterval { get; }
        public string ConsumerGroup { get; }
    }
}