namespace Rydo.Kafka.Client.Benchmark
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;
    using Dispatchers;
    using FakeData;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serializations.Serializers;

    [MemoryDiagnoser]
    [NativeMemoryProfiler]
    public class MessageDispatcherValueStringSerializerBenchmark
    {
        private const int N = 10_000;
        private ServiceProvider? _serviceProvider;
        private ILogger<MessageDispatcherValueStringSerializer>? _logger;

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            _serviceProvider = services.BuildServiceProvider();
            _logger = _serviceProvider.GetRequiredService<ILogger<MessageDispatcherValueStringSerializer>>();
        }

        [Benchmark]
        public void SerializerGetMessageValue()
        {
            var serializer = new MessageDispatcherValueStringSerializer(_logger);

            for (var counter = 0; counter < N; counter++)
            {
                var request = ProducerRequest.Create("topic", new DummyModel
                {
                    Id = counter,
                    Name = $"NAME-{counter}"
                });

                var messageValue = serializer.GetMessageValue(request);
            }
        }
    }
}