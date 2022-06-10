namespace Rydo.Kafka.Client.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Configurations;
    using Constants;
    using Handlers;

    internal static class ConsumersSpecificationExtensions
    {
        public static void AddConsumers<TKey, TValue>(this IKafkaClientConfigurator<TKey, TValue> configurator,
            IEnumerable<TopicDefinition> consumerTopicDefinitions, IEnumerable<Assembly> assemblies)
        {
            var consumerHandlers =
                new Dictionary<(string, string), (Type? ContractType, Type? HandlerType)>();

            foreach (var exportedTypes in assemblies.Select(x => x.ExportedTypes))
            {
                foreach (var exportedType in exportedTypes)
                {
                    if (!TryGetConsumerHandler(exportedType, out var consumerHandlerType))
                        continue;

                    var consumerHandlerId = (exportedType.Assembly.GetName().Name, exportedType.FullName!);
                    consumerHandlers.Add(consumerHandlerId, consumerHandlerType);
                }
            }

            configurator.Consumer.Configure((services, consumerContext) =>
            {
                foreach (var consumerTopicDefinition in consumerTopicDefinitions)
                {
                    foreach (var consumerHandler in consumerHandlers.Where(messageHandler =>
                                 IsTopicConsumerAttribute(consumerTopicDefinition.TopicName, messageHandler)))
                    {
                        consumerContext.AddConsumer(consumerTopicDefinition, consumerHandler.Value);
                    }
                }
            });

            configurator.Producer.Configure((services, producerContext) =>
            {
                foreach (var consumerTopicDef in consumerTopicDefinitions)
                {
                    var topicDeadLetter = TopicConventions.GetTopicDeadLetter(consumerTopicDef.TopicName);

                    var consumerTopicDeadLetterDef = new TopicConsumerDefinition(topicDeadLetter,
                        consumerTopicDef.ConsumerGroup, consumerTopicDef.CryptKey,
                        consumerTopicDef.RetryAttempts, consumerTopicDef.RetryInterval);

                    producerContext.AddProducer(consumerTopicDef);
                    producerContext.AddProducer(consumerTopicDeadLetterDef);
                }
            });

            static bool TryGetConsumerHandler(Type type,
                out (Type? ContractType, Type? HandlerType) consumerHandlerType)
            {
                consumerHandlerType = default;

                if (!type.GetInterfaces().Any(FindConsumerHandler))
                    return false;

                var clientContract = type
                    .GetInterfaces()
                    .Where(x => x.IsGenericType && typeof(IConsumerHandler).IsAssignableFrom(x))
                    .Select(x => x.GenericTypeArguments[0]).FirstOrDefault();

                consumerHandlerType = (clientContract, type);

                return true;
            }
        }

        private static bool IsTopicConsumerAttribute(string topicName,
            KeyValuePair<(string, string), (Type? ContractType, Type? HandlerType)> clientTypes)
        {
            if (clientTypes.Value.HandlerType is null)
                throw new Exception();

            return clientTypes.Value.HandlerType
                .CustomAttributes.FirstOrDefault(x => x.AttributeType.Name.Contains(nameof(TopicConsumerAttribute)))!
                .ConstructorArguments.Any(x =>
                    x.Value != null &&
                    x.Value.ToString().Equals(topicName, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool FindConsumerHandler(Type type)
        {
            var a = typeof(IConsumerHandler<>);

            return type.IsInterface
                   && type.IsGenericType
                   && type.Name.Contains(nameof(IConsumerHandler), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}