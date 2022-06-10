namespace Rydo.Kafka.Client.Benchmark
{
    using System.Text;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;

    [MemoryDiagnoser]
    [NativeMemoryProfiler]
    public class StringAllocationSample
    {
        private const int N = 1_000;

        [Benchmark]
        public string Contact()
        {
            var s = "";
            for (var counter = 0; counter < N; counter++)
            {
                s += "lorem ipsum";
            }

            return s;
        }
        
        [Benchmark]
        public string StringBuilder()
        {
            var s = new StringBuilder();
            for (var counter = 0; counter < N; counter++)
            {
                s.Append("lorem ipsum");
            }

            return s.ToString();
        }
    }
}