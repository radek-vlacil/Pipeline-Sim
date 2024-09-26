using System.Diagnostics.Metrics;

namespace TrafficSim.Processors
{
    internal class HedgingProcessor<T> : Processor<T>
    {
        private readonly int _hedgeDelay;
        private readonly IProcessor<T> _primary;
        private readonly IProcessor<T> _secondary;
        private readonly ITaskQueue _taskQueue;

        private class Status
        {
            public bool Done { get; set; }
        }

        public HedgingProcessor(string name, int hedgeDelay, IProcessor<T> primary, IProcessor<T> secondary, IClock clock, ITaskQueue queue, IMeterFactory f)
            : base(name, clock, f)
        {
            _hedgeDelay = hedgeDelay;
            _primary = primary;
            _secondary = secondary;
            _taskQueue = queue;
        }   

        protected async override Task<Result> HandleProcessAsync(T args)
        {
            var delay = _taskQueue.EnqueueAsync(_hedgeDelay);
            var primary = _primary.ProcessAsync(args);

            var first = await Task.WhenAny(delay, primary);

            if (first == primary)
            {
                var result = await primary;
                if (result == Result.Success)
                {
                    return result;
                }
                else
                {
                    return await _secondary.ProcessAsync(args);
                }
            }
            else
            {
                var secondary = _secondary.ProcessAsync(args);
                var firstFinished = await Task.WhenAny(primary, secondary);
                var result = await firstFinished;

                if (result == Result.Success)
                {
                    return result;
                }
                else
                {
                    return await ((firstFinished == primary) ? secondary : primary);
                }
            }
        }
    }
}
