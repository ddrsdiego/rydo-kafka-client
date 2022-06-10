namespace Rydo.Kafka.Client.Models
{
    using System;

    public sealed class Topic
    {
        public Topic(string name)
        {
            Name = name.ToUpperInvariant() ?? throw new ArgumentNullException(nameof(name));
        }

        public readonly string Name;
        
        public override string ToString() => Name;
    }
}