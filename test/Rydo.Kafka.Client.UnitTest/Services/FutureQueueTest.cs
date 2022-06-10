namespace Rydo.Kafka.Client.UnitTest.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Client.Services;
    using FluentAssertions;
    using Xunit;

    public class FutureQueueTest
    {
        [Fact]
        public async Task Should_Run_FutureTask_With_Result_Success_2()
        {
            const string errorMessage = "some-error-message";
            
            var futureQueue = FutureQueue.CreateQueueDefault();
            var res = await futureQueue.Run(async () =>
            {
                await Task.Delay(500);
                throw new Exception(errorMessage);

                return true;
            });

            res.IsSuccess.Should().BeFalse();
            res.IsFailure.Should().BeTrue();
        }
        
        [Fact]
        public async Task Should_Run_With_Error_And_Return_Failure()
        {
            const string errorMessage = "some-error-message";
            
            var futureQueue = FutureQueue.CreateQueueDefault();
            var res = await futureQueue.Run(async () =>
            {
                await Task.Delay(500);
                throw new Exception(errorMessage);
            });

            res.IsSuccess.Should().BeFalse();
            res.IsFailure.Should().BeTrue();
            res.Error.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Should_Run_FutureTask_With_Result_Success()
        {
            const string expected = "Success()";

            var futureQueue = FutureQueue.CreateQueueDefault();
            var res = await futureQueue.Run(async () => await AsyncMethodWithResult(expected));

            res.IsSuccess.Should().BeTrue();
            res.IsFailure.Should().BeFalse();
        }

        [Fact]
        public async Task Test_1()
        {
            var futureQueue = FutureQueue.CreateQueueDefault();
            var res = await futureQueue.Run(async () => await AsyncMethodWithoutResult());

            res.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Push_FutureTask_Without_Result()
        {
            var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            var futureQueue = FutureQueue.CreateQueueDefault();
            await futureQueue.Push(async () =>
            {
                await AsyncMethodWithoutResult();
                completion.TrySetResult();
            }, CancellationToken.None);

            await completion.Task;
            completion.Task.IsCompletedSuccessfully.Should().BeTrue();
        }

        private static async Task AsyncMethodWithoutResult() => await Task.Delay(1_000);

        private static async Task<string> AsyncMethodWithResult(string result)
        {
            await Task.Delay(1_000);
            return result;
        }
    }
}