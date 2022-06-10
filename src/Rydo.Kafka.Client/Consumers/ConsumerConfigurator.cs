namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Middlewares.Extensions;
    using Serializations.Extensions;
    using Services;

    internal sealed class ConsumerConfigurator : IConsumerConfigurator<byte[], byte[]>
    {
        private readonly IServiceCollection _services;
        private readonly IConsumerContextContainer<byte[], byte[]> _consumerContextContainer;
        private readonly IKafkaListenerContainer<byte[], byte[]> _kafkaListenerStringContainer;
        
        public ConsumerConfigurator(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));

            _consumerContextContainer = new ConsumerContextContainer();
            _kafkaListenerStringContainer = new KafkaListenerContainer();
        }

        public void Configure(Action<IServiceCollection, IConsumerContextContainer<byte[], byte[]>> specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            specification(_services, _consumerContextContainer);

            foreach (var (topicName, kafkaConsumerContext) in _consumerContextContainer.Entries)
            {
                if (kafkaConsumerContext == null)
                    continue;

                var kafkaListener = new KafkaListener(topicName, kafkaConsumerContext);

                _services.TryAddScoped(kafkaConsumerContext.HandlerType ?? throw new InvalidOperationException());
                _kafkaListenerStringContainer.AddListener(topicName, kafkaListener);
            }

            _services.AddSingleton(serviceProvider =>
            {
                _kafkaListenerStringContainer.SetServiceProvider(serviceProvider);
                return _kafkaListenerStringContainer;
            });

            _services.AddMiddlewares();
            _services.AddDeserializers<byte[]>();
            _services.AddSingleton(_consumerContextContainer);
            _services.AddHostedService<KafkaIntegrationHostedService>();
        }
    }
}