namespace Rydo.Kafka.Client.Handlers
{
    using System;

    public interface IReceivedContext
    {
        DateTime ReceivedAt { get; }
        string? ConsumerName { get; }
    }

    internal sealed class ReceivedContext : IReceivedContext
    {
        public ReceivedContext(string? consumerName)
        {
            ConsumerName = consumerName;
            ReceivedAt = DateTime.UtcNow;
        }

        public DateTime ReceivedAt { get; }
        public string? ConsumerName { get; }
    }
}