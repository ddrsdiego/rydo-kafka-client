namespace Rydo.Kafka.Client.Models
{
    public class GroupId
    {
        public GroupId(string value)
        {
            Value = value;
        }
        
        public string Value { get; }

        public override string ToString() => Value;
    }
}