namespace Rydo.Kafka.Client.Logging.Message
{
    using System.Text;
    using Confluent.Kafka;

    internal abstract class KafkaProducerLog : KafkaMessageLog
    {
        protected KafkaProducerLog(string log, string? cid, string? environment, DeliveryResult<byte[], byte[]>?
            deliveryResult)
            : base(log, cid, environment, Encoding.UTF8.GetString(deliveryResult.Message.Key), deliveryResult.Topic,
                deliveryResult.Partition, deliveryResult.Offset)
        {
        }
    }
}