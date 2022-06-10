namespace Rydo.Kafka.Client.Producers
{
    using System;
    using Models;

    public class ProducerSpecification
    {
        public ProducerSpecification(string topicName, string cryptKey)
        {
            if (topicName == null) throw new ArgumentNullException(nameof(topicName));
            
            CryptKey = cryptKey;
            Topic = new Topic(topicName);
        }

        public readonly Topic Topic;
        public readonly string CryptKey;
    }
}