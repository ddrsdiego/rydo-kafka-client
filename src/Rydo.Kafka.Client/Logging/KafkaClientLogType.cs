namespace Rydo.Kafka.Client.Logging
{
    public static class KafkaClientLogType
    {
        public const string MessageIn = "in-message";
        public const string MessageOut = "out-message";
        public const string CommitRecord = "commit-record";
        public const string StartingConsumer = "STARTING-CONSUMER";
    }
}