using System.Diagnostics.Metrics;

namespace TrafficSim
{
    internal abstract class Processor<T> : IProcessor<T>
    {
        private readonly UpDownCounter<int> _requestActive;
        private readonly Counter<int> _requestNew;
        private readonly Counter<int> _requestDone;
        private readonly Histogram<int> _latencies;

        protected IClock Clock { get; init; }

        public string Name { get; init; }

        protected Processor(string name, IClock clock, IMeterFactory factory)
        {
            Name = name;
            var meter = factory.Create(name); 
            _requestActive = meter.CreateUpDownCounter<int>("active");
            _requestNew = meter.CreateCounter<int>("new");
            _requestDone = meter.CreateCounter<int>("done");
            _latencies = meter.CreateHistogram<int>("latencies");
            Clock = clock;
        }

        public async Task<Result> ProcessAsync(T args)
        {
            NewRequest();

            var startTime = Clock.Now;
            var result = await HandleProcessAsync(args);

            RequestDone(result, startTime, args);

            return result;
        }

        protected abstract Task<Result> HandleProcessAsync(T args);

        private void NewRequest()
        {
            _requestActive.Add(1);
            _requestNew.Add(1);
        }

        protected void RequestDone(Result result, Time startTime, T args)
        {
            _requestActive.Add(-1);
            _requestDone.Add(1);
            _latencies.Record(Clock.Now - startTime,
                new KeyValuePair<string, object?>("args", args?.ToString() ?? "<null>"),
                new KeyValuePair<string, object?>("result", result.ToString()));
        }
    }
}
