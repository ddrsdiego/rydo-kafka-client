namespace Rydo.Kafka.Client.Logging.Message
{
    internal abstract class KafkaMessageLog
    {
        protected KafkaMessageLog(string log, string? cid, string? environment, string messageKey, string topic,
            int par, long offset)
        {
            Log = log;
            Cid = cid;
            Environment = environment;
            MessageKey = messageKey;
            Topic = topic;
            Par = par;
            Offset = offset;
        }

        public string Log { get; }
        public string? Environment { get; }
        public string? Cid { get; }
        public string MessageKey { get; }
        public string Topic { get; }
        public int Par { get; }
        public long Offset { get; }
    }
}