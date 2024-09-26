using System.Diagnostics.Metrics;

namespace TrafficSim.Processors
{
    internal class TimeoutProcessor<T> : Processor<T>
    {
        private readonly ITaskQueue _taskQueue;
        private readonly int _timeout;
        private readonly IProcessor<T> _processor;

        public TimeoutProcessor(string name, int timeout, IProcessor<T> processor, IClock clock, ITaskQueue taskQueue, IMeterFactory f)
            : base(name, clock, f)
        {
            _timeout = timeout;
            _taskQueue = taskQueue;
            _processor = processor;
        }

        protected override async Task<Result> HandleProcessAsync(T args)
        {
            var timeout = _taskQueue.EnqueueAsync(_timeout);
            var process = _processor.ProcessAsync(args);
            var result = await Task.WhenAny(timeout, process);

            if (result == timeout)
            {
                return Result.Failure;
            }
            return Result.Success;
        }
    }
}
