namespace Rydo.Kafka.Client.UnitTest.Shared
{
    using System;
    using System.Threading.Tasks;
    using Handlers;

    [TopicConsumer("consumer-test")]
    public class ConsumerHandlerTest : ConsumerHandler<DummyModel>
    {
        private readonly object _syncLock;

        public ConsumerHandlerTest()
        {
            _syncLock = new object();
        }

        public override Task Consumer(MessageConsumerContext context)
        {
            var counter = 0;
            Parallel.ForEach(context.ConsumerRecords, (consumerRecord) =>
            {
                lock (_syncLock)
                {
                    counter++;
                }

                if (counter % 2 == 0)
                {
                    context.ConsumerRecords.MarkToRetry(consumerRecord, "TEST", new Exception());
                }
            });

            return Task.CompletedTask;
        }
    }
}