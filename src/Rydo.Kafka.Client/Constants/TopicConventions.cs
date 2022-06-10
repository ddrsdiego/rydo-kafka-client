namespace Rydo.Kafka.Client.Constants
{
    public static class TopicConventions
    {
        private const string DeadLetterSuffix = "_DEADLETTER";

        public static string GetTopicDeadLetter(string topicName) => $"{topicName}{DeadLetterSuffix}";
    }
}