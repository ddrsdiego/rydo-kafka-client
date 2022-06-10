namespace Rydo.Kafka.Client.UnitTest.Serializations
{
    using System;
    using Client.Dispatchers;
    using Client.Serializations.Deserializers;
    using Client.Serializations.Serializers;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Shared;
    using Xunit;
    public class MessageDispatcherValueStringSerializerTest
    {
        private readonly ILogger<MessageDispatcherValueStringSerializer> _loggerSerializer;
        private readonly ILogger<MessageDispatcherValueStringDeserializer> _loggerDeserializer;

        public MessageDispatcherValueStringSerializerTest()
        {
            _loggerSerializer = NSubstitute.Substitute.For<ILogger<MessageDispatcherValueStringSerializer>>();
            _loggerDeserializer = NSubstitute.Substitute.For<ILogger<MessageDispatcherValueStringDeserializer>>();
        }

        [Fact]
        public void Test()
        {
            //arrange
            var model = new DummyModel
            {
                Id = 1,
                Name = "DUMMY-NAME",
                BirthDate = new DateTime(1982, 11, 22)
            };

            var key = Guid.NewGuid().ToString();
            var serializer = new MessageDispatcherValueStringSerializer(_loggerSerializer);
            var deserializer = new MessageDispatcherValueStringDeserializer(_loggerDeserializer);

            var request = ProducerRequest.Create("dummy-topic-name", key, model);

            //act
            var payload = serializer.GetMessageValue(request);

            //assert
            var kafkaEnvelope = deserializer.GetMessageValue<KafkaEnvelope>(payload);
            kafkaEnvelope?.Key.Should().Be(key);
        }

        [Fact]
        public void Test_1()
        {
            //arrange
            var model = new DummyModel {Id = 1, Name = "DUMMY-MODEL"};
            
            var key = Guid.NewGuid().ToString();
            var serializer = new MessageDispatcherValueStringSerializer(_loggerSerializer);
            var deserializer = new MessageDispatcherValueStringDeserializer(_loggerDeserializer);

            var request = ProducerRequest.Create("dummy-topic-name", key, model);

            //act
            var payload = serializer.GetMessageValue(request);

            //assert
            var kafkaEnvelope = deserializer.GetMessageValue<KafkaEnvelope>(payload);
            kafkaEnvelope?.Key.Should().Be(key);
        }
    }
}