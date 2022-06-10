namespace Rydo.Kafka.Client.UnitTest.FakeData
{
    using System.Collections.Generic;
    using Shared;

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