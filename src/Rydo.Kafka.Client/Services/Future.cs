namespace Rydo.Kafka.Client.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Future : IFuture
    {
        private readonly Func<Task> _method;
        private readonly CancellationToken _cancellationToken;
        private readonly TaskCompletionSource<bool> _completion;

        public Future(Func<Task> method, CancellationToken cancellationToken)
        {
            _method = method;
            _cancellationToken = cancellationToken;
            _completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public Task Completed => _completion.Task;
        
        public async Task Run()
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _completion.TrySetCanceled(_cancellationToken);
                return;
            }

            try
            {
                await _method().ConfigureAwait(false);
                _completion.TrySetResult(true);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == _cancellationToken)
            {
                _completion.TrySetCanceled(exception.CancellationToken);
            }
            catch (Exception exception)
            {
                _completion.TrySetException(exception);
            }
        }
    }

    internal class Future<T> : IFuture
    {
        private readonly Func<Task<T>> _method;
        private readonly CancellationToken _cancellationToken;
        private readonly TaskCompletionSource<T> _completion;

        public Future(Func<Task<T>> method, CancellationToken cancellationToken)
        {
            _method = method;
            _cancellationToken = cancellationToken;
            _completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public Task<T> Completed => _completion.Task;

        public async Task Run()
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _completion.TrySetCanceled(_cancellationToken);
                return;
            }

            try
            {
                var task = _method().ConfigureAwait(false);
                var result = await task;

                _completion.TrySetResult(result);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == _cancellationToken)
            {
                _completion.TrySetCanceled(exception.CancellationToken);
            }
            catch (Exception exception)
            {
                _completion.TrySetException(exception);
            }
        }
    }
}