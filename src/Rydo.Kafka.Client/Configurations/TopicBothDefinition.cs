namespace Rydo.Kafka.Client.Configurations
{
    using Constants;

    internal sealed class TopicBothDefinition : TopicDefinition
    {
        public TopicBothDefinition(string topicName, string consumerGroup, string cryptKey, int retryAttempts = 0, string retryInterval = "")
            : base(topicName, TopicDirections.Both, cryptKey, consumerGroup, retryAttempts, retryInterval)
        {
        }
    }
}