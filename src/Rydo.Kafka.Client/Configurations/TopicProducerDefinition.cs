namespace Rydo.Kafka.Client.Configurations
{
    using Constants;

    internal sealed class TopicProducerDefinition : TopicDefinition
    {
        public TopicProducerDefinition(string topicName, string cryptKey)
            : base(topicName, TopicDirections.Producer, cryptKey, string.Empty, 0)
        {
        }
    }
}