namespace Rydo.Kafka.Client.UnitTest.Dispatchers
{
    using Shared;
    using Xunit;

    public class KafkaMessageTemplateTest
    {
        [Fact]
        public void Test()
        {
            var message = new DummyModel
            {
                Id = 1, Name = "NAME-MODEL"
            };
            
            
        }
    }
}