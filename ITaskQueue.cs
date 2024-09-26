namespace TrafficSim
{
    internal interface ITaskQueue
    {
        public void Enqueue(int duration, Action action);

        public Task EnqueueAsync(int duration);

        public void Run(Time time);
    }
}
