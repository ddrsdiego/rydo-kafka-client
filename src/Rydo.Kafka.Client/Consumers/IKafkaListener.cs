namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IKafkaListener<TKey, TValue>
    {
        string TopicName { get; }

        int FaultCounter { get; }

        Task<bool> IsRunning { get; set; }

        void SetupDependencies(IServiceProvider? dependencyResolver);
        
        Task<bool> StartAsync(CancellationToken stoppingToken);
        
        void IncrementFaultCounter();
    }
}