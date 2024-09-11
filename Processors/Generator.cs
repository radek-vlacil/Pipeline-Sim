namespace TrafficSim.Processors
{
    internal class Generator<T> : Processor<T>
    {
        private readonly int _requests;
        private readonly IProcessor<T> _processor;

        public Generator(string name, int requests, IProcessor<T> processor, IClock clock)
            : base(name, clock)
        {
            _requests = requests;
            _processor = processor;
        }

        public void Generate(T args)
        {
            for (var i = 0; i < _requests; i++)
            {
                ProcessAsync(args);
            }
        }

        protected async override Task<Result> HandleProcessAsync(T args)
        {
            return await _processor.ProcessAsync(args);
        }
    }
}
