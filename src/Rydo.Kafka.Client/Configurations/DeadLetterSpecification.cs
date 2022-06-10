namespace Rydo.Kafka.Client.Configurations
{
    using System;

    public class DeadLetterSpecification
    {
        public DeadLetterSpecification(int attempts, TimeSpan interval)
        {
            Retry = new TopicDeadLetterReplaySpec(attempts, interval);
        }

        public TopicDeadLetterReplaySpec Retry { get; }
    }
}