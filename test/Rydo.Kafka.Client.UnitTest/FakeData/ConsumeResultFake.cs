namespace Rydo.Kafka.Client.UnitTest.FakeData
{
    using System;
    using System.Text;
    using Client.Dispatchers;
    using Client.Serializations.Serializers;
    using Confluent.Kafka;
    using Microsoft.Extensions.Logging;
    using Shared;

    public static partial class Fake
    {
        public static ConsumeResult<byte[], byte[]> GetConsumeResult(DummyModel model, int partition = 0,
            int offset = 0)
        {
            const string topicName = "dummy-topic";
            const string groupId = "dummy-topic-group-id";

            var loggerSerializer = NSubstitute.Substitute.For<ILogger<MessageDispatcherValueUtf8BytesSerializer>>();
            var serializer = new MessageDispatcherValueUtf8BytesSerializer(loggerSerializer);

            var key = Guid.NewGuid().ToString();
            var request = ProducerRequest.Create("dummy-topic-name", key, model);
            var messageValue = serializer.GetMessageValue(request);

            var headers = new Headers();
            
            var consumeResult = new ConsumeResult<byte[], byte[]>
            {
                TopicPartitionOffset =
                    new TopicPartitionOffset(topicName, new Partition(partition), new Offset(offset)),
                
                Message = new Message<byte[], byte[]>
                {
                    Value = messageValue,
                    Key = Encoding.UTF8.GetBytes(model.Id.ToString()),
                    Headers = headers
                },
            };
            return consumeResult;
        }
    }
}