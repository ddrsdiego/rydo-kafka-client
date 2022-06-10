namespace Rydo.Kafka.Client.Configurations
{
    using Constants;

    internal sealed class TopicConsumerDefinition : TopicDefinition
    {
        public TopicConsumerDefinition(string topicName, string consumerGroup, string cryptKey, int retryAttempts = 0,
            string retryInterval = "")
            : base(topicName, TopicDirections.Consumer, cryptKey, consumerGroup, retryAttempts, retryInterval)
        {
        }
    }
}