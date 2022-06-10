namespace Rydo.Kafka.Client.Configurations
{
    public sealed class TopicDeadLetterEntry
    {
        public DeadLetterRetryEntry? Replay { get; set; }
    }
}