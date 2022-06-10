namespace Rydo.Kafka.Client.Configurations.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Constants;

    internal static class TopicConfigExtractExtensions
    {
        public static IEnumerable<TopicDefinition> ExtractProducersDefs(this ITopicConfigContextContainer topicConfigContextContainer)
            => topicConfigContextContainer.Entries.Values
                .Where(FindProducersSpecs)
                .Select(topicSpecification => topicSpecification);

        public static IEnumerable<TopicDefinition> ExtractConsumersDefs(this ITopicConfigContextContainer topicConfigContextContainer)
            => topicConfigContextContainer.Entries.Values
                .Where(FindConsumerSpecs)
                .Select(topicSpecification => topicSpecification);

        private static bool FindProducersSpecs(TopicDefinition specification) =>
            specification.Direction.Equals(TopicDirections.Both, StringComparison.InvariantCultureIgnoreCase) ||
            specification.Direction.Equals(TopicDirections.Producer, StringComparison.InvariantCultureIgnoreCase);

        private static bool FindConsumerSpecs(TopicDefinition specification) =>
            specification.Direction.Equals(TopicDirections.Both, StringComparison.InvariantCultureIgnoreCase) ||
            specification.Direction.Equals(TopicDirections.Consumer, StringComparison.InvariantCultureIgnoreCase);
    }
}