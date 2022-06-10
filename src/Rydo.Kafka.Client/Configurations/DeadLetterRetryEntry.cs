namespace Rydo.Kafka.Client.Configurations
{
    public sealed class DeadLetterRetryEntry
    {
        public int Attempts { get; set; }
        public string? Interval { get; set; }
    }
}