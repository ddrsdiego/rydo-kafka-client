namespace Rydo.Kafka.Client.Dispatchers
{
    using Confluent.Kafka;
    using CSharpFunctionalExtensions;

    public interface IKafkaMessageFactory<TType>
    {
        Result<Message<TType, TType>> CreateKafkaMessage(ProducerRequest producerRequest);
    }
}