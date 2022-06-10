namespace Rydo.Kafka.Client.Services
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using CSharpFunctionalExtensions;

    public interface IFutureQueue : IAsyncDisposable
    {
        ValueTask Push(Func<Task> method, CancellationToken cancellationToken = default);

        ValueTask Push<T>(Func<Task<T>> method, CancellationToken cancellationToken = default);

        Task<Result> Run(Func<Task> method, CancellationToken cancellationToken = default);

        Task<Result<T>> Run<T>(Func<Task<T>> method, CancellationToken cancellationToken = default);
    }

    internal class FutureQueue : IFutureQueue
    {
        private readonly Task _readerTask;
        private readonly Channel<IFuture> _channel;

        private FutureQueue(int queueCapacity)
        {
            var channelOptions = new BoundedChannelOptions(queueCapacity)
            {
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            _channel = Channel.CreateBounded<IFuture>(channelOptions);
            _readerTask = Task.Run(ConsumeQueue);
        }

        public static IFutureQueue CreateQueueDefault() => new FutureQueue(1);

        public ValueTask Push(Func<Task> method, CancellationToken cancellationToken = default)
        {
            var future = new Future(method, cancellationToken);

            var writeTask = WriteChannel(future, cancellationToken);

            return writeTask.IsCompletedSuccessfully ? new ValueTask() : SlowWrite(writeTask);
        }

        public ValueTask Push<T>(Func<Task<T>> method, CancellationToken cancellationToken = default)
        {
            var future = new Future<T>(async () => await method(), cancellationToken);

            var writeTask = WriteChannel(future, cancellationToken);

            return writeTask.IsCompletedSuccessfully ? new ValueTask() : SlowWrite(writeTask);
        }

        public async Task<Result> Run(Func<Task> method, CancellationToken cancellationToken = default)
        {
            var future = new Future(async () => await method().ConfigureAwait(false), cancellationToken);
            
            _ = WriteChannel(future, cancellationToken).ConfigureAwait(false);

            try
            {
                await future.Completed;
                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Failure(e.ToString());
            }
        }

        public async Task<Result<T>> Run<T>(Func<Task<T>> method, CancellationToken cancellationToken = default)
        {
            var future = new Future<T>(async () => await method().ConfigureAwait(false), cancellationToken);

            _ = WriteChannel(future, cancellationToken).ConfigureAwait(false);
            
            try
            {
                var res = await future.Completed;
                return Result.Success(res);
            }
            catch (Exception e)
            {
                return Result.Failure<T>(e.ToString());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ValueTask WriteChannel(IFuture future, CancellationToken cancellationToken = default)
        {
            var writeTask = _channel.Writer.WriteAsync(future, cancellationToken);
            return writeTask.IsCompletedSuccessfully ? new ValueTask() : SlowWrite(writeTask);
        }

        private static async ValueTask SlowWrite(ValueTask slowTask) => await slowTask;

        private async Task ConsumeQueue()
        {
            while (await _channel.Reader.WaitToReadAsync().ConfigureAwait(false))
            {
                if (!_channel.Reader.TryRead(out var future))
                    continue;

                try
                {
                    await future.Run();
                }
                catch (OperationCanceledException exception)
                {
                    var error = exception.Message;
                }
                catch (Exception exception)
                {
                    var error = exception.Message;
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            _channel.Writer.Complete();
            await _readerTask;
        }
    }
}