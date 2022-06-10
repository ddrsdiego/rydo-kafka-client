namespace Rydo.Kafka.Client.Configurations
{
    using System;

    public class TopicDeadLetterReplaySpec
    {
        public TopicDeadLetterReplaySpec(int attempts, TimeSpan interval)
        {
            Attempts = attempts;
            Interval = interval;
        }

        public int Attempts { get; }
        public TimeSpan Interval { get; }
    }
}