namespace Rydo.Kafka.Client.Exceptions
{
    using System;

    public class ProducerContextNotFoundException : Exception
    {
        public ProducerContextNotFoundException(string producerTopicName)
            : base(CreateMessage(producerTopicName))
        {
        }
        
        private static string CreateMessage(string producerTopicName) =>
            $"The context for the {producerTopicName} producer was not defined in the configurations. {producerTopicName} is correct?";
    }
}