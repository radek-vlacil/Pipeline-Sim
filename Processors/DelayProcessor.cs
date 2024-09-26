using System.Diagnostics.Metrics;

namespace TrafficSim.Processors
{
    internal class DelayProcessor<T> : Processor<T>
    {
        private readonly List<(Time, int)> _configuration;
        private readonly IProcessor<T> _processor;
        private readonly ITaskQueue _taskQueue;

        public DelayProcessor(string name, List<(Time, int)> configuration, IProcessor<T> processor, IClock clock, ITaskQueue queue, IMeterFactory f)
            : base(name, clock, f)
        {
            _configuration = new List<(Time, int)>(configuration);
            _configuration.Reverse();
            _processor = processor;
            _taskQueue = queue;
        }

        public DelayProcessor(string name, int delay, IProcessor<T> processor, IClock clock, ITaskQueue queue, IMeterFactory f)
            : this(name, [(Time.Min, delay)], processor, clock, queue, f)
        {
        }

        protected async override Task<Result> HandleProcessAsync(T args)
        {
            await _taskQueue.EnqueueAsync(GetDelay());

            return await _processor.ProcessAsync(args);
        }

        private int GetDelay()
        {
            var now = Clock.Now;
            foreach (var (time, delay) in _configuration)
            {
                if (time <= now)
                {
                    return delay;
                }
            }
            return _configuration[0].Item2;
        }
    }
}
