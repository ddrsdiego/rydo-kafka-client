namespace Rydo.Kafka.Client.Configurations
{
    using System.ComponentModel.DataAnnotations;

    public class TopicConfig
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Direction { get; set; }
        
        public string CryptKey { get; set; }
        
        public string ConsumerGroup { get; set; }
        
        public string DeadLetterPolicyName { get; set; }
    }
}