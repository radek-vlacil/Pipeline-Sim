namespace TrafficSim.Processors
{
    internal class RateProcessor<T> : Processor<T>
    {
        private readonly IProcessor<T> _processor;
        private readonly int _rate;
        private int t = 0;
        private int bucket = 0;

        public RateProcessor(string name, int rate, IProcessor<T> processor, IClock clock)
            : base(name, clock)
        {
            _processor = processor;
            _rate = rate;
            bucket = rate;
        }

        protected override Task<Result> HandleProcessAsync(T args)
        {
            if (t < Clock.Now)
            {
                t = Clock.Now;
                bucket = _rate; 
            }

            if (bucket > 0)
            {
                bucket--;
                return _processor.ProcessAsync(args);
            }
            else
            {
                return Task.FromResult(Result.Throttled);
            }
        }
    }
}
