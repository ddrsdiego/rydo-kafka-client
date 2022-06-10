namespace Rydo.Kafka.Client.Exceptions
{
    using System;

    public class ConsumerErrorException : Exception
    {
        public ConsumerErrorException(string consumerName, string message)
            : base(message)
        {
            ConsumerName = consumerName;
        }

        public string ConsumerName { get; }
    }
}