
using System.Diagnostics.Metrics;

namespace TrafficSim.Processors
{
    internal class LinearProcessor<T> : Processor<T>
    {
        private readonly List<IProcessor<T>> _processors;
        private readonly Func<Result, bool> _exitCondition;

        public LinearProcessor(string name, List<IProcessor<T>> processors, Func<Result, bool> exitCondition, IClock clock, IMeterFactory f)
            : base(name, clock, f)
        {
            _processors = processors;
            _exitCondition = exitCondition;
        }

        protected async override Task<Result> HandleProcessAsync(T args)
        {
            Result result = Result.Success;
            foreach (var p in _processors)
            {
                result = await p.ProcessAsync(args);
                if (_exitCondition(result))
                {
                    return result;
                }
            }

            return result;
        }
    }
}
