namespace Rydo.Kafka.Client.Benchmark.FakeData
{
    using System.Text;
    using System.Text.Json;
    using Confluent.Kafka;
    using Serializations;

    public static partial class Fake
    {
        public static ConsumeResult<byte[], byte[]> GetConsumeResult(DummyModel model, int partition = 0,
            int offset = 0)
        {
            const string topicName = "dummy-topic";
            const string groupId = "dummy-topic-group-id";

            var json = JsonSerializer.Serialize(model, SystemTextJsonMessageSerializer.Options);

            var consumeResult = new ConsumeResult<byte[], byte[]>
            {
                TopicPartitionOffset =
                    new TopicPartitionOffset(topicName, new Partition(partition), new Offset(offset)),
                Message = new Message<byte[], byte[]>
                {
                    Value = Encoding.UTF8.GetBytes(json),
                    Key = Encoding.UTF8.GetBytes(model.Id.ToString()),
                }
            };
            return consumeResult;
        }
    }
}