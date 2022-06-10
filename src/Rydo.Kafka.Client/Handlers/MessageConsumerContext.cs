namespace Rydo.Kafka.Client.Handlers
{
    using System;
    using System.Threading;
    using Confluent.Kafka;
    using Consumers;
    using Dispatchers;

    public sealed class MessageConsumerContext
    {
        public MessageConsumerContext(IConsumerRecords consumerRecords,
            ConsumerContext<byte[], byte[]>? consumerContext,
            IReceivedContext received,
            IMessageDispatcher? dispatcher,
            IConsumer? consumer,
            CancellationToken cancellationToken)
        {
            ConsumerRecords = consumerRecords;
            Received = received;
            Dispatcher = dispatcher;
            Consumer = consumer;
            CancellationToken = cancellationToken;
            HandlerType = consumerContext?.HandlerType;

        }
        
        internal readonly Type? HandlerType;
        internal readonly IConsumer? Consumer;
        
        public readonly IMessageDispatcher? Dispatcher;
        public readonly IConsumerRecords ConsumerRecords;
        public readonly IReceivedContext Received;
        public readonly CancellationToken CancellationToken;
    }
}