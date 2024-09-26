namespace TrafficSim
{
    internal interface IProcessor
    {
        string Name { get; }
    }

    internal interface IProcessor<T> : IProcessor
    {
        Task<Result> ProcessAsync(T args);
    }
}
