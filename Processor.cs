namespace TrafficSim
{
    internal abstract class Processor<T> : IProcessor<T>
    {
        private int _requestActive;
        private int _requestNew;
        private int _requestDone;
        private int _requestFailed;
        private int _requestSuccessful;
        private List<int> _latencies;

        protected IClock Clock { get; init; }

        public string Name { get; init; }

        protected Processor(string name, IClock clock)
        {
            Name = name;
            _requestActive = 0;
            _requestNew = 0;
            _requestDone = 0;
            _requestFailed = 0;
            _requestSuccessful = 0;
            _latencies = [];
            Clock = clock;
        }

        public void DumpStats(StreamWriter output)
        {
            var average = _latencies.Count > 0 ? _latencies.Average() : 0;
            var max = _latencies.Count > 0 ? _latencies.Max() : 0;

            output.WriteLineAsync($"{Clock.Now},{Name},{_requestActive},{_requestNew},{_requestDone},{_requestFailed},{_requestSuccessful},{average},{max}");
            output.Flush();
            ResetStats();
        }

        public void PrintTitle(StreamWriter output)
        {
            output.WriteLineAsync("Time, Name, Active, New, Done, Failed, Successful, LatencyAvg, LatencyMax");
        }

        public async Task<Result> ProcessAsync(T args)
        {
            NewRequest();

            var startTime = Clock.Now;
            var result = await HandleProcessAsync(args);

            RequestDone(result, startTime);

            return result;
        }

        protected abstract Task<Result> HandleProcessAsync(T args);

        private void NewRequest()
        {
            _requestActive++;
            _requestNew++;
        }

        protected void RequestDone(Result result, Time startTime)
        {
            _requestActive--;
            _requestDone++;
            _latencies.Add(Clock.Now - startTime);
            if (result == Result.Success)
            {
                _requestSuccessful++;
            }
            else
            {
                _requestFailed++;
            }
        }

        private void ResetStats()
        {
            _requestNew = 0;
            _requestDone = 0;
            _requestFailed = 0;
            _requestSuccessful = 0;
            _latencies = [];
        }
    }
}
