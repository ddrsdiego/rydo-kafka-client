namespace Rydo.Kafka.Client.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurations;
    using Configurations.DependencyInjection;
    using Configurations.Extensions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Constants;
    using Consumers;
    using Dispatchers;

    public static class AsyncServicesConfigurationExtensions
    {
        public static IServiceCollection AddAsyncServices(this IServiceCollection services,
            IConfiguration configuration, params Type[] types)
        {
            const string asyncServicesTopics = "AsyncServices:Topics";
            const string asyncServicesDeadLetterPolicies = "AsyncServices:DeadLetterPolicies";

            var topicConfigs = configuration.GetSection(asyncServicesTopics).Get<TopicConfig[]>();
            if (topicConfigs is null || !topicConfigs.Any())
                throw new InvalidOperationException();

            var deadLetterPolicyEntry = new DeadLetterPolicyEntry();
            var section = configuration.GetSection(asyncServicesDeadLetterPolicies);
            section.Bind(deadLetterPolicyEntry.Entries);

            CheckConsumerConfigIsValid(types, topicConfigs);

            var context = new TopicConfigContextContainer();
            context.AddTopicConfig(topicConfigs, deadLetterPolicyEntry, types.FirstOrDefault());

            var dependencyConfigurator = new MicrosoftDependencyConfigurator(services);
            
            services.AddAsyncServices<byte[], byte[]>(configurator =>
            {
                configurator.AddProducers(context.ExtractProducersDefs());
                configurator.AddConsumers(context.ExtractConsumersDefs(), types.Select(x => x.Assembly));
            });
            
            services.TryAddSingleton<ITopicConfigContextContainer>(context);
            services.TryAddSingleton<IConsumerErrorHandler, ConsumerErrorHandler>();
            
            return services;
        }

        private static void CheckConsumerConfigIsValid(Type[] types, IEnumerable<TopicConfig> topicConfigs)
        {
            if (!topicConfigs.Any(x =>
                    x.Direction.ToLowerInvariant().Equals(TopicDirections.Both) ||
                    x.Direction.ToLowerInvariant().Equals(TopicDirections.Consumer))) return;

            if (types is null || types.Length == 0)
                throw new InvalidOperationException();
        }

        private static IServiceCollection AddAsyncServices<TKey, TValue>(this IServiceCollection services,
            Action<IKafkaClientConfigurator<byte[], byte[]>> configurator)
        {
            var stilingueConfigurator = new KafkaClientConfigurator(services);
            configurator(stilingueConfigurator);

            services.AddScoped<IMessageDispatcher, KafkaMessageDispatcher>();
            return services;
        }
    }
}