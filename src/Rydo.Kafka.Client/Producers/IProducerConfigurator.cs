namespace Rydo.Kafka.Client.Producers
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public interface IProducerConfigurator<TKey, TValue>
    {
        void Configure(Action<IServiceCollection, IProducerContextContainer<TKey, TValue>> specification);
    }
}