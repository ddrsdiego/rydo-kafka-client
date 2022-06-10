namespace Rydo.Kafka.Client.Consumers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    public interface IConsumeResultHandle<TKey,TValue>
    {
        Task Handle(ConsumeResult<TKey, TValue> consumeResult, CancellationToken cancellationToken = default);
    }
}