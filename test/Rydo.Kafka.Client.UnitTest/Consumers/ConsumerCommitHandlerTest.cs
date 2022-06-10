// namespace Rydo.Kafka.Client.UnitTest.Consumers
// {
//     using System.Threading.Tasks;
//     using Confluent.Kafka;
//     using FakeData;
//     using Microsoft.Extensions.Logging;
//     using NSubstitute;
//     using Xunit;
//
//     public class ConsumerCommitHandlerTest
//     {
//         private readonly IConsumer<byte[], byte[]> _consumer;
//         private readonly ILogger<ConsumerCommitHandler> _logger;
//
//         public ConsumerCommitHandlerTest()
//         {
//             _consumer = Substitute.For<IConsumer<byte[], byte[]>>();
//             _logger = Substitute.For<ILogger<ConsumerCommitHandler>>();
//         }
//
//         [Fact]
//         public Task Should_Execute_Commit_With_Success()
//         {
//             const string topicName = "dummy-topic";
//             const string groupId = "dummy-topic-group-id";
//
//             //arrange
//
//             var consumerCommitHandler = new ConsumerCommitHandler(_logger);
//             consumerCommitHandler.SetConsumer(_consumer);
//
//             var consumerRecords = Fake.GetConsumerRecords(10, 1_000);
//
//             //act
//             var task = consumerCommitHandler.Commit(null);
//
//             //assert
//             task.IsCompletedSuccessfully.Should().BeTrue();
//
//             return task.AsTask();
//         }
//     }
// }