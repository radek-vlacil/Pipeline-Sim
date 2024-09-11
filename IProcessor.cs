namespace TrafficSim
{
    internal interface IProcessor
    {
        string Name { get; }

        void PrintTitle(StreamWriter output);
        void DumpStats(StreamWriter output);
    }

    internal interface IProcessor<T> : IProcessor
    {
        Task<Result> ProcessAsync(T args);
    }
}
