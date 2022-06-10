namespace Rydo.Kafka.Client.Exceptions
{
    using System;

    public class ProducerErrorException : Exception
    {
        public ProducerErrorException(string consumerName, string message)
            : base(message)
        {
            ConsumerName = consumerName;
        }

        public string ConsumerName { get; }
    }
}