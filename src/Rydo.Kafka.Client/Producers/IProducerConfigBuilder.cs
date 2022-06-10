namespace Rydo.Kafka.Client.Producers
{
    using Confluent.Kafka;

    public interface IProducerConfigBuilder
    {
        /// <summary>
        ///     This field indicates the number of acknowledgements the leader broker must receive from ISR brokers
        ///     before responding to the request: Zero=Broker does not send any response/ack to client, One=The
        ///     leader will write the record to its local log but will respond without awaiting full acknowledgement
        ///     from all followers. All=Broker will block until message is committed by all in sync replicas (ISRs).
        ///     If there are less than min.insync.replicas (broker configuration) in the ISR set the produce request
        ///     will fail.
        /// </summary>
        /// <param name="acks"></param>
        /// <returns></returns>
        IProducerConfigBuilder Acks(Acks acks);

        ProducerConfig GetProducerConfig();
    }
}