namespace Rydo.Kafka.Client.Benchmark
{
    using System.Reflection;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            // var b = new ConsumerCommitHandlerBenchmark();
            // b.Setup();
            //
            // await b.ConsumerCommitHandlerDictionary();
            //
            // Task task = SleepAsync();
            // DumpThread("Before first await");
            // await task;
            // DumpThread("Before second await");
            // await task;
            // DumpThread("The end");
            
            var r = BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
        }

        private static async Task SleepAsync()
        {
            DumpThread("SleepAsync started");
            
            await Task.Delay(1_000);
            
            DumpThread("SleepAsync ended");
        }

        private static void DumpThread(string label)
        {
            Console.WriteLine($"[{DateTime.Now:hh:mm:ss.fff}] {label}: TID: {Environment.CurrentManagedThreadId}");
        }
    }
}