namespace Rydo.Kafka.Client.Producers
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Serializations.Extensions;

    internal sealed class ProducerConfigurator : IProducerConfigurator<byte[], byte[]>
    {
        private readonly IServiceCollection _services;
        private readonly IProducerContextContainer<byte[], byte[]> _producerContextContainer;

        public ProducerConfigurator(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _producerContextContainer = new ProducerContextContainer(_services);
        }

        public void Configure(Action<IServiceCollection, IProducerContextContainer<byte[], byte[]>> specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            specification(_services, _producerContextContainer);
            _services.AddSingleton(_producerContextContainer);
            
            _services.AddSerializers<byte[]>();
            _services.AddSingleton<IProducerErrorHandler, ProducerErrorHandler>();
        }
    }
}