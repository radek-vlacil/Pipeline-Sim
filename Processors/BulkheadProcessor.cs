namespace TrafficSim.Processors
{
    internal class BulkheadProcessor<T> : Processor<T>
    {
        private readonly int _maxQueued;
        private volatile int _queued;
        private readonly IProcessor<T> _processor;
        private readonly SemaphoreSlim _semaphore;

        public BulkheadProcessor(string name, int maxConcurrent, int maxQueued, IProcessor<T> processor, IClock clock)
            : base(name, clock)
        {
            _maxQueued = maxQueued;
            _processor = processor;
            _semaphore = new SemaphoreSlim(maxConcurrent);
        }

        protected async override Task<Result> HandleProcessAsync(T args)
        {
            if (_queued >= _maxQueued)
            {
                return Result.Failure;
            }
            ++_queued;

            await _semaphore.WaitAsync();
            try
            {
                var result = await _processor.ProcessAsync(args);
                --_queued;
                return result;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
