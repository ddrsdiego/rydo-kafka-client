namespace Rydo.Kafka.Client.Benchmark.FakeData
{
    using System.Collections.Generic;

    public static partial class Fake
    {
        public static IEnumerable<DummyModel> GetDummyModels(int amount)
        {
            var models = new List<DummyModel?>(amount);

            for (var counter = 0; counter < amount; counter++)
            {
                models.Add(new DummyModel
                {
                    Id = counter, Name = $"NAME-{counter}"
                });
            }
            
            return models;
        }
    }
}