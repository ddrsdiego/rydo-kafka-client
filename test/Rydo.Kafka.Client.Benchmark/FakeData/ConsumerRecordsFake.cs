namespace Rydo.Kafka.Client.Benchmark.FakeData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Consumers;

    public static partial class Fake
    {
        public static IConsumerRecords GetConsumerRecords(int partitionsCounter, int offsetCounter)
        {
            const string topicName = "dummy-topic";
            const string groupId = "dummy-topic-group-id";

            var models = GetDummyModels(offsetCounter);
            
            var syncLock = new object();
            var consumerRecords = new ConsumerRecords();
            
            Parallel.For(0, partitionsCounter, (partition, state) =>
            {
                var offset = 0;
                foreach (var model in models)
                {
                    lock (syncLock)
                    {
                        var consumeResult = GetConsumeResult(model, partition, offset);
                        var consumerContext = GetConsumerContext();

                        consumerRecords.Add(new ConsumerRecord(model, consumeResult, consumerContext,
                            new Dictionary<string, object>()));

                        offset++;
                    }
                }
            });

            return consumerRecords;
        }
    }
}