namespace Rydo.Kafka.Client.UnitTest.Consumers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Client.Consumers;
    using Client.Serializations;
    using Confluent.Kafka;
    using FluentAssertions;
    using Handlers;
    using Shared;
    using Xunit;

    public class ConsumerRecordsTest
    {
        [Fact]
        public async Task Test()
        {
            const string dummyTopic = "dummy-topic";

            var models = new List<DummyModel?>();
            IConsumerRecords consumerRecords = new ConsumerRecords();

            for (var counter = 0; counter < 10; counter++)
            {
                models.Add(new DummyModel
                {
                    Id = counter,
                    Name = $"NAME-{counter}"
                });
            }

            var offset = 0;
            foreach (var model in models)
            {
                var json = JsonSerializer.Serialize(model, SystemTextJsonMessageSerializer.Options);

                if (model == null) continue;

                var consumeResult = new ConsumeResult<byte[], byte[]>
                {
                    TopicPartitionOffset =
                        new TopicPartitionOffset(dummyTopic, new Partition(0), new Offset(offset)),
                    Message = new Message<byte[], byte[]>
                    {
                        Key = Encoding.UTF8.GetBytes(model.Id.ToString()),
                        Value = Encoding.UTF8.GetBytes(json)
                    }
                };

                consumerRecords.Add(new ConsumerRecord(model, consumeResult, FakeData.Fake.GetConsumerContext(),
                    new Dictionary<string, object>()));

                offset++;
            }

            var consumerHandlerTest = new ConsumerHandlerTest();

            await consumerHandlerTest.Consumer(
                new MessageConsumerContext(consumerRecords, FakeData.Fake.GetConsumerContext(),
                    new ReceivedContext("consumer-context-test"), default, default,
                    CancellationToken.None));

            consumerRecords.HasFaults.Should().BeTrue();
            consumerRecords.Faults.Count().Should().Be(5);
        }
    }
}