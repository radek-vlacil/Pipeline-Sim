using System.Diagnostics.Metrics;

namespace TrafficSim.Processors
{
    internal class ServiceProcessor<T> : LinearProcessor<T>
    {
        public ServiceProcessor(string name, List<IProcessor<T>> processors, IClock clock, IMeterFactory f)
            : base(name, processors, (result) => result == Result.Failure, clock, f)
        {
        }
    }
}
