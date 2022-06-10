namespace Rydo.Kafka.Client.Benchmark.FakeData
{
    using System.Collections.Generic;
    using Confluent.Kafka;
    using Consumers;

    public static partial class Fake
    {
        public static ConsumerContext<byte[], byte[]> GetConsumerContext()
        {
            const string topicName = "dummy-topic";
            const string groupId = "dummy-topic-group-id";
            
            var consumerContext = new ConsumerContext<byte[], byte[]>(
                new ConsumerSpecification(topicName, groupId, 3, "1s"),
                new ConsumerConfig(),
                new ConsumerBuilder<byte[], byte[]>(new List<KeyValuePair<string, string>>()),
                typeof(DummyModel),
                typeof(ConsumerHandlerTest));
            return consumerContext;
        }
    }
}