namespace Rydo.Kafka.Client.UnitTest.FakeData
{
    using System;
    using System.Collections.Generic;
    using Client.Consumers;
    using Confluent.Kafka;
    using Moq;
    using Shared;

    public static partial class Fake
    {
        public static ConsumerContext<byte[], byte[]> GetConsumerContext()
        {
            const string topicName = "dummy-topic";
            const string groupId = "dummy-topic-group-id";

            var a = new Mock<ConsumerBuilder<byte[], byte[]>>(MockBehavior.Loose);
            a.Setup(x => x.Build()).Returns(() =>
            {
                var c = new Mock<IConsumer<byte[], byte[]>>();
                return c.Object;
            });

            var consumerBuilder = new ConsumerBuilder<byte[], byte[]>(new ConsumerConfig
            {
                BootstrapServers = "localhost:666",
                GroupId = Guid.NewGuid().ToString(),
            });

            var consumerContext = new ConsumerContext<byte[], byte[]>(
                new ConsumerSpecification(topicName, groupId, 3, "1s"),
                new ConsumerConfig(),
                consumerBuilder,
                typeof(DummyModel),
                typeof(ConsumerHandlerTest));
            return consumerContext;
        }
    }
}