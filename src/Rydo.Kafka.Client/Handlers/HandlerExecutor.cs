namespace Rydo.Kafka.Client.Handlers
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    internal abstract class HandlerExecutor
    {
        private static readonly ConcurrentDictionary<Type, HandlerExecutor> Executors =
            new ConcurrentDictionary<Type, HandlerExecutor>();

        public abstract Task Execute(object handler, MessageConsumerContext context, object message);

        public static HandlerExecutor GetExecutor(Type messageType)
        {
            return Executors.GetOrAdd(
                messageType,
                _ => (HandlerExecutor) Activator.CreateInstance(
                    typeof(IConsumerHandler<>).MakeGenericType(messageType)));
        }

        private class InnerHandlerExecutor<T> : HandlerExecutor
        {
            public override Task Execute(object handler, MessageConsumerContext context, object message)
            {
                var h = (IConsumerHandler<T>) handler;

                return h.Consumer(context);
            }
        }
    }
}