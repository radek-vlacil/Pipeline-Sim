namespace TrafficSim.Processors
{
    internal class StatusProcessor<T> : Processor<T>
    {
        private readonly List<(Time, int)> _configuration;

        public StatusProcessor(string name, List<(Time, int)> configuration, IClock clock)
            : base(name, clock)
        {
            _configuration = configuration;
            _configuration.Reverse();
        }

        public StatusProcessor(string name, int failureRate, IClock clock)
            : this(name, [(Time.Min, failureRate)], clock)
        {
        }

        public StatusProcessor(string name, Result result, IClock clock)
            : this(
                  name,
                  [(Time.Min, result == Result.Failure ? 100 : 0)],
                  clock)
        {
        }

        protected override Task<Result> HandleProcessAsync(T args)
        {
            return Random.Shared.Next(0, 99) < GetFailureRate() ? 
                 Task.FromResult(Result.Failure)
                : Task.FromResult(Result.Success);
        }

        private int GetFailureRate()
        {
            var now = Clock.Now;
            foreach (var (time, failureRate) in _configuration)
            {
                if (time <= now)
                {
                    return failureRate;
                }
            }
            return _configuration[0].Item2;
        }
    }
}
