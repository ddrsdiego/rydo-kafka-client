namespace Rydo.Kafka.Client.UnitTest.Dispatchers
{
    using System;
    using System.Text;
    using Client.Dispatchers;
    using Client.Serializations.Serializers;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Shared;
    using Xunit;

    public class KafkaUtf8BytesMessageFactoryTest
    {
        private readonly ILogger<KafkaUtf8BytesMessageFactory> _logger;
        private readonly IMessageDispatcherSerializer<byte[], byte[]> _serializer;

        public KafkaUtf8BytesMessageFactoryTest()
        {
            var loggerKey = Substitute.For<ILogger<MessageDispatcherKeyUtf8BytesSerializer>>();
            var loggerValue = Substitute.For<ILogger<MessageDispatcherValueUtf8BytesSerializer>>();

            _logger = Substitute.For<ILogger<KafkaUtf8BytesMessageFactory>>();
            _serializer = new MessageDispatcherUtf8BytesSerializer(
                new MessageDispatcherKeyUtf8BytesSerializer(loggerKey),
                new MessageDispatcherValueUtf8BytesSerializer(loggerValue));
        }

        [Fact]
        public void Should_Create_Message_With_Request_Automatic_Key()
        {
            //arrange
            var request =
                ProducerRequest.Create("dummy-topic", new DummyModel
                {
                    Id = 1,
                    Name = "NAME-FAKE"
                });
            var kafkaMessageFactory = new KafkaUtf8BytesMessageFactory(_logger, _serializer);

            //act
            var response = kafkaMessageFactory.CreateKafkaMessage(request);

            //assert
            response.IsSuccess.Should().BeTrue();
            Encoding.UTF8.GetString(response.Value.Key).Should().Be(request.MessageKey);
        }

        [Fact]
        public void Should_Create_Message_With_Request_Provide_Key()
        {
            //arrange
            var key = Guid.NewGuid().ToString();
            var request =
                ProducerRequest.Create("dummy-topic", key, new DummyModel
                {
                    Id = 1,
                    Name = "NAME-FAKE"
                });
            var kafkaMessageFactory = new KafkaUtf8BytesMessageFactory(_logger, _serializer);

            //act
            var response = kafkaMessageFactory.CreateKafkaMessage(request);

            //assert
            response.IsSuccess.Should().BeTrue();
            Encoding.UTF8.GetString(response.Value.Key).Should().Be(request.MessageKey);
        }

        [Fact]
        public void Should_Create_Message_With_Request_Provide_Key_1()
        {
            //arrange
            var key = Guid.NewGuid().ToString();
            var request =
                ProducerRequest.Create("dummy-topic", key, new DummyModel
                {
                    Id = 1,
                    Name = "NAME-FAKE"
                });
            var kafkaMessageFactory = new KafkaUtf8BytesMessageFactory(_logger, _serializer);

            //act
            var response = kafkaMessageFactory.CreateKafkaMessage(default);

            //assert
            response.IsFailure.Should().BeTrue();
        }
    }
}