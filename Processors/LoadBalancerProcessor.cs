using System.Diagnostics.Metrics;

namespace TrafficSim.Processors
{
    internal class LoadBalancerProcessor<T> : Processor<T>
    {
        private readonly List<IProcessor<T>> _processors;
        private int _index;

        public LoadBalancerProcessor(string name, List<IProcessor<T>> processors, IClock clock, IMeterFactory f)
            : base(name, clock, f)
        {
            _processors = processors;
            _index = 0;
        }

        protected override async Task<Result> HandleProcessAsync(T args)
        {
            return await _processors[_index++].ProcessAsync(args);
        }
    }
}
