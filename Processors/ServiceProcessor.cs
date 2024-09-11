namespace TrafficSim.Processors
{
    internal class ServiceProcessor<T> : LinearProcessor<T>
    {
        public ServiceProcessor(string name, List<IProcessor<T>> processors, IClock clock)
            : base(name, processors, (result) => result == Result.Failure, clock)
        {
        }
    }
}
