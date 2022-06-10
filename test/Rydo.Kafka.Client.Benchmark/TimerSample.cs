namespace Rydo.Kafka.Client.Benchmark
{
    using BenchmarkDotNet.Attributes;

    public class TimerSample
    {
        [Benchmark]
        public void Fast() => Thread.Sleep(50);
        
        [Benchmark]
        public void Normal() => Thread.Sleep(100);
        
        [Benchmark]
        public void Slow() => Thread.Sleep(150);
    }
}