namespace Rydo.Kafka.Client.Configurations.Extensions
{
    using System;
    using Constants;

    internal static class TopicConfigAdapterExtension
    {
        public static TopicDefinition? AdapterConfigToDefinition(this TopicConfig topicConfig, Type? type)
        {
            return topicConfig.AdapterConfigToDefinition(new DeadLetterPolicyItem
            {
                Retry = DeadLetterPolicyItem.EmptyRetry
            }, type);
        }

        public static TopicDefinition AdapterConfigToDefinition(this TopicConfig topicConfig,
            DeadLetterPolicyItem? deadLetterPolicyItem, Type? type)
        {
            var consumerGroup = string.Empty;

            if (!topicConfig.Direction.Equals(TopicDirections.Producer, StringComparison.InvariantCultureIgnoreCase))
            {
                consumerGroup = string.IsNullOrEmpty(topicConfig.ConsumerGroup)
                    ? GetConsumerGroupDefault(topicConfig, type)
                    : topicConfig.ConsumerGroup;
            }

            TopicDefinition? topicDefinition = topicConfig.Direction switch
            {
                TopicDirections.Producer => new TopicProducerDefinition(topicConfig.Name, topicConfig.CryptKey),

                TopicDirections.Consumer => new TopicConsumerDefinition(topicConfig.Name, consumerGroup,
                    topicConfig.CryptKey, deadLetterPolicyItem.Retry.Attempts, deadLetterPolicyItem.Retry.Interval),

                TopicDirections.Both => new TopicBothDefinition(topicConfig.Name, consumerGroup, topicConfig.CryptKey,
                    deadLetterPolicyItem.Retry.Attempts, deadLetterPolicyItem.Retry.Interval),
                _ => throw new ArgumentOutOfRangeException()
            };

            return topicDefinition;
        }

        private static string GetConsumerGroupDefault(TopicConfig topicConfig, Type? type)
        {
            var consumerGroupPrefix = type?.Assembly.GetName().Name?.ToUpperInvariant();
            
            var consumerGroupDefault = $"{consumerGroupPrefix}-{topicConfig.Name.ToUpperInvariant()}";
            return consumerGroupDefault;
        }
    }
}