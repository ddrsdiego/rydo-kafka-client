namespace Rydo.Kafka.Client.Exceptions
{
    using System;

    public class TopicSpecAlreadyDefinedException : Exception
    {
        public TopicSpecAlreadyDefinedException(string serviceName, string topicName)
            : base(CreateMessage(serviceName, topicName))
        {
        }

        private static string CreateMessage(string serviceName, string topicName) =>
            $"The {topicName} topic for the {serviceName} service has been previously defined in the settings file.";
    }
}