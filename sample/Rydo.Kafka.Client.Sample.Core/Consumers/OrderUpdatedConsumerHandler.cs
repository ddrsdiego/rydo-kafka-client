namespace Rydo.Kafka.Client.Sample.Core.Consumers
{
    using System.Threading.Tasks;
    using Handlers;
    using Models;

    [TopicConsumer("ORDER-UPDATED")]
    public class OrderUpdatedConsumerHandler : ConsumerHandler<OrderUpdated>
    {
        public override Task Consumer(MessageConsumerContext context) => Task.CompletedTask;
    }
}