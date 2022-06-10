namespace Rydo.Kafka.Client.UnitTest.Serializations
{
    using System;
    using System.Text.Json;
    using Client.Dispatchers;
    using Client.Serializations.Deserializers;
    using Client.Serializations.Serializers;
    using FluentAssertions;
    using MassTransit.Serialization;
    using Microsoft.Extensions.Logging;
    using Shared;
    using Xunit;

    public class MessageDispatcherValueUtf8BytesSerializerTest
    {
        private readonly ILogger<MessageDispatcherValueUtf8BytesSerializer> _loggerSerializer;
        private readonly ILogger<MessageDispatcherValueUtf8BytesDeserializer> _loggerDeserializer;

        public MessageDispatcherValueUtf8BytesSerializerTest()
        {
            _loggerSerializer = NSubstitute.Substitute.For<ILogger<MessageDispatcherValueUtf8BytesSerializer>>();
            _loggerDeserializer = NSubstitute.Substitute.For<ILogger<MessageDispatcherValueUtf8BytesDeserializer>>();
        }

        [Fact]
        public void Test_1()
        {
            //arrange
            var model = new DummyModel {Id = 1, Name = "DUMMY-MODEL"};

            var key = Guid.NewGuid().ToString();
            var serializer = new MessageDispatcherValueUtf8BytesSerializer(_loggerSerializer);
            var deserializer = new MessageDispatcherValueUtf8BytesDeserializer(_loggerDeserializer);

            var request = ProducerRequest.Create("dummy-topic-name", key, model);

            //act
            var payload = serializer.GetMessageValue(request);

            //assert
            var kafkaEnvelope = deserializer.GetMessageValue<KafkaEnvelope>(payload);
            kafkaEnvelope?.Key.Should().Be(key);
            
            var modelDeserialized =
                JsonSerializer.Deserialize<DummyModel>(kafkaEnvelope?.Payload, SystemTextJsonMessageSerializer.Options);
        }
    }
}