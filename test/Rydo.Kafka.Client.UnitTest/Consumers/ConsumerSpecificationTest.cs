namespace Rydo.Kafka.Client.UnitTest.Consumers
{
    using System;
    using Client.Consumers;
    using FluentAssertions;
    using Xunit;

    public class ConsumerSpecificationTest
    {
        [Theory]
        [InlineData(3, "1s", 3, 1)]
        [InlineData(10, "59s", 10, 59)]
        public void Should_Create_ConsumerSpec_With_Retry_Seconds(int retryAttempts, string retryInterval,
            int retryAttemptsExpected, int retryIntervalExpected)
        {
            var consumerSpecification =
                new ConsumerSpecification("topic-name", "group-id", retryAttempts, retryInterval);

            consumerSpecification.DeadLetter?.Retry.Attempts.Should().Be(retryAttemptsExpected);
            consumerSpecification.DeadLetter?.Retry.Interval.Should().Be(TimeSpan.FromSeconds(retryIntervalExpected));
        }

        [Theory]
        [InlineData(3, "1m", 3, 1)]
        [InlineData(10, "59m", 10, 59)]
        [InlineData(10, "69m", 10, 69)]
        public void Should_Create_ConsumerSpec_With_Retry_Minutes(int retryAttempts, string retryInterval,
            int retryAttemptsExpected, int retryIntervalExpected)
        {
            var consumerSpecification =
                new ConsumerSpecification("topic-name", "group-id", retryAttempts, retryInterval);

            consumerSpecification.DeadLetter?.Retry.Attempts.Should().Be(retryAttemptsExpected);
            consumerSpecification.DeadLetter?.Retry.Interval.Should().Be(TimeSpan.FromMinutes(retryIntervalExpected));
        }

        [Theory]
        [InlineData(3, "1h", 3, 1)]
        [InlineData(10, "59h", 10, 59)]
        [InlineData(10, "69h", 10, 69)]
        public void Should_Create_ConsumerSpec_With_Retry_Hours(int retryAttempts, string retryInterval,
            int retryAttemptsExpected, int retryIntervalExpected)
        {
            var consumerSpecification =
                new ConsumerSpecification("topic-name", "group-id", retryAttempts, retryInterval);

            consumerSpecification.DeadLetter?.Retry.Attempts.Should().Be(retryAttemptsExpected);
            consumerSpecification.DeadLetter?.Retry.Interval.Should().Be(TimeSpan.FromHours(retryIntervalExpected));
        }

        [Fact]
        public void Should_Create_ConsumerSpec_With_Retry_HOurs_1()
        {
            //

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var consumerSpecification =
                    new ConsumerSpecification("topic-name", "group-id", 3, "12");    
            });
        }
        
        [Fact]
        public void Should_Create_ConsumerSpec_Without_DeadLetterSpec()
        {
            var consumerSpecificationWithNoRetry = new ConsumerSpecification("topic-name", "group-id");
            consumerSpecificationWithNoRetry.DeadLetter.Should().BeNull();
        }

        [Fact]
        public void Should_Throw_Exception_When_TopicName_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var consumer = new ConsumerSpecification(string.Empty, "group-id");
            });
        }
        
        [Fact]
        public void Should_Throw_Exception_When_GroupId_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var consumer = new ConsumerSpecification("topic-name", string.Empty);
            });
        }
        
        [Fact]
        public void Should_Throw_Exception_When_TopicName_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var consumer = new ConsumerSpecification(null, "group-id");
            });
        }
        
        [Fact]
        public void Should_Throw_Exception_When_GroupId_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var consumer = new ConsumerSpecification("topic-name", null);
            });
        }
    }
}