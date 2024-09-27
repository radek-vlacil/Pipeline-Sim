
using System.Diagnostics.Metrics;

namespace TrafficSim.Processors
{
    internal class RetryProcessor<T> : Processor<T>
    {
        private readonly IProcessor<T> _processor;
        private readonly int _maxTries;
        private readonly int _initialDelay;
        private readonly ITaskQueue _taskQueue;
        private readonly Func<T, Result, bool> _shouldRetry;

        public RetryProcessor(string name, IProcessor<T> processor, int maxRetries, int initialDelay, Func<T, Result, bool> shouldRetry, IClock clock, ITaskQueue taskQueue, IMeterFactory f)
            : base(name, clock, f)
        {
            _processor = processor;
            _maxTries = maxRetries + 1;
            _initialDelay = initialDelay;
            _taskQueue = taskQueue;
            _shouldRetry = shouldRetry ?? ((i, j) => j != Result.Success);
        }

        protected async override Task<Result> HandleProcessAsync(T args)
        {
            Result result = Result.Failure;
            var tryCount = 0;
            while (tryCount < (_maxTries))
            {
                ++tryCount;
                result = await _processor.ProcessAsync(args);
                if (!_shouldRetry(args, result) || tryCount >= (_maxTries))
                {
                    return result;
                }
                //await _taskQueue.EnqueueAsync(GetDelay(tryCount));
            }
            return result;
        }

        private int GetDelay(int retry)
        {
            return _initialDelay * GetPower(retry);
        }

        private static int GetPower(int pow)
        {
            var ret = 1;
            for (int i = 0; i < pow; i++)
            {
                ret *= 2;
            }

            return ret;
        }
    }
}
