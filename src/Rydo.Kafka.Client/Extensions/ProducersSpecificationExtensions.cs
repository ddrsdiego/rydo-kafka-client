namespace Rydo.Kafka.Client.Extensions
{
    using System.Collections.Generic;
    using Configurations;

    public static class ProducersSpecificationExtensions
    {
        public static void AddProducers<TKey, TValue>(this IKafkaClientConfigurator<TKey, TValue> configurator,
            IEnumerable<TopicDefinition> producerTopicDefinitions)
        {
            configurator.Producer.Configure((services, producerContext) =>
            {
                foreach (var producerTopicDefinition in producerTopicDefinitions)
                {
                    producerContext.AddProducer(producerTopicDefinition);
                }
            });
        }
    }
}