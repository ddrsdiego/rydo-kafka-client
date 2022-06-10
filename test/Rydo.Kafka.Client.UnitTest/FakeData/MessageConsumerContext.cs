namespace Rydo.Kafka.Client.UnitTest.FakeData
{
    using System.Threading;
    using Client.Dispatchers;
    using Confluent.Kafka;
    using Handlers;
    using NSubstitute;

    public static partial class Fake
    {
        public static MessageConsumerContext GetMessageConsumerContext(int partitionCounter, int offsetCounter)
        {
            var consumer = Substitute.For<IConsumer<byte[], byte[]>>();
            var messageDispatcher = Substitute.For<IMessageDispatcher>();

            var consumerContext = GetConsumerContext();
            var consumerRecords = GetConsumerRecords(partitionCounter, offsetCounter);
            var receiveContext = new ReceivedContext("consumer-name");

            var messageConsumerContext = new MessageConsumerContext(consumerRecords, consumerContext, receiveContext,
                messageDispatcher, default,
                CancellationToken.None);

            return messageConsumerContext;
        }
    }
}