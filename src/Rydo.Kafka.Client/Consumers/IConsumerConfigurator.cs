namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public interface IConsumerConfigurator<TKey, TValue>
    {
        void Configure(Action<IServiceCollection, IConsumerContextContainer<TKey, TValue>> specification);
    }
}