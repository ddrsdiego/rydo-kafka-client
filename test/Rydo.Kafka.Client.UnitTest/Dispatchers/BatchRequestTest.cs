namespace Rydo.Kafka.Client.UnitTest.Dispatchers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rydo.Kafka.Client.Dispatchers;
    using Shared;
    using Xunit;

    public class BatchRequestTest
    {
        private const string? TopicName = "dummy-topic-name";

        [Fact]
        public void Should_Throw_Exception_When_Value_Is_Null()
        {
            var request = ProducerBatchRequest.Create(TopicName);
            Assert.Throws<ArgumentNullException>(() => request.Add(null));
        }

        [Fact]
        public void Should_Throw_Exception_When_Key_And_Value_Are_Invalid()
        {
            var request = ProducerBatchRequest.Create(TopicName);
            Assert.Throws<ArgumentNullException>(() => request.Add(string.Empty, null));
        }

        [Fact]
        public void Should_Throw_Exception_When_Value_Is_Null_And_Key_Is_Valid()
        {
            var request = ProducerBatchRequest.Create(TopicName);
            Assert.Throws<ArgumentNullException>(() => request.Add(Guid.NewGuid().ToString(), null));
        }

        [Fact]
        public void Should_Throw_Exception_When_TopicName_Is_Empty() =>
            Assert.Throws<ArgumentNullException>(() => ProducerBatchRequest.Create(string.Empty));

        [Fact]
        public void Should_Throw_Exception_When_TopicName_Is_Null() =>
            Assert.Throws<ArgumentNullException>(() => ProducerBatchRequest.Create(null));

        [Fact]
        public void Should_Add_One_Model_Without_Key()
        {
            const string? topicName = "dummy-topic-name";

            var request = ProducerBatchRequest.Create(topicName);
            request.Add(new DummyModel());

            request.Count.Should().Be(1);
            request.TopicName.Should().Be(topicName.ToUpperInvariant());

            var item = request.Items.FirstOrDefault();
            item.MessageKey.Should().NotBeNull();
        }

        [Fact]
        public void Should_Add_One_Model_With_Key()
        {
            const string? topicName = "dummy-topic-name";

            var messageKey = Guid.NewGuid().ToString().Split('-')[0];

            var request = ProducerBatchRequest.Create(topicName);
            request.Add(messageKey, new DummyModel());

            request.Count.Should().Be(1);
            request.TopicName.Should().Be(topicName.ToUpperInvariant());

            request.Items.FirstOrDefault().MessageKey.Should().NotBeNull();
            request.Items.FirstOrDefault().MessageKey.Should().Be(messageKey);
        }

        [Fact]
        public void Should_Add_Ten_Models_With_Key()
        {
            const int capacity = 10;
            const string? topicName = "dummy-topic-name";

            var messages = new Dictionary<string, DummyModel>();
            var request = ProducerBatchRequest.Create(topicName);

            for (var counter = 0; counter < capacity; counter++)
            {
                messages.Add(counter.ToString(), new DummyModel { Id = counter, Name = $"NAME-{counter}" });
            }

            foreach (var (key, dummyModel) in messages)
            {
                request.Add(key, dummyModel);
            }

            request.Count.Should().Be(capacity);
            request.Items.Count.Should().Be(capacity);
            request.TopicName.Should().Be(topicName.ToUpperInvariant());

            var item = request.Items.FirstOrDefault(x =>
                x.MessageKey.Equals("1", StringComparison.InvariantCultureIgnoreCase));
            item.Should().NotBeNull();
            item.MessageKey.Should().Be("1");
        }

        [Fact]
        public void Should_Add_Ten_Models_In_Concurrency_Model()
        {
            const int capacity = 1_000;
            const string? topicName = "dummy-topic-name";

            var messages = new Dictionary<string, DummyModel>();
            var request = ProducerBatchRequest.Create(topicName);

            for (var counter = 0; counter < capacity; counter++)
            {
                messages.Add(counter.ToString(), new DummyModel { Id = counter, Name = $"NAME-{counter}" });
            }

            Parallel.ForEach(messages, (message) =>
            {
                var (key, value) = message;
                request.Add(key, value);
            });

            request.Count.Should().Be(capacity);
            request.Items.Count.Should().Be(capacity);
            request.TopicName.Should().Be(topicName.ToUpperInvariant());

            for (var counter = 0; counter < capacity; counter++)
            {
                var item = request.Items.FirstOrDefault(x =>
                    x.MessageKey.Equals(counter.ToString(), StringComparison.InvariantCultureIgnoreCase));

                item.Should().NotBeNull();
            }
        }
    }
}