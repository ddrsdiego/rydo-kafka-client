namespace Rydo.Kafka.Client.Consumers
{
    public enum AutoOffsetReset
    {
        /// <summary>Only reads new messages in the topic</summary>
        Latest,

        /// <summary>Reads the topic from the beginning</summary>
        Earliest,
    }
}