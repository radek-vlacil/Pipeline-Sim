using System.Collections.Concurrent;

namespace TrafficSim
{
    internal class TaskQueue : SynchronizationContext, ITaskQueue
    {
        private readonly PriorityQueue<Action, int> _timeQueue;
        private readonly ConcurrentQueue<(SendOrPostCallback, object?)> _workQueue;
        private readonly IClock _clock;

        public TaskQueue(IClock clock)
        {
            _clock = clock;
            _timeQueue = new PriorityQueue<Action, int>();
             _workQueue = new ConcurrentQueue<(SendOrPostCallback, object?)>();
        }

        public void Enqueue(int duration, Action action)
        {
            _timeQueue.Enqueue(action, _clock.Now + duration);
        }

        public Task EnqueueAsync(int duration)
        {
            var task = new TaskCompletionSource<bool>();
            Enqueue(duration, () => task.SetResult(true));
            return task.Task;
        }

        public override void Post(SendOrPostCallback d, object? state)
        {
            _workQueue.Enqueue((d, state));
        }

        public override void Send(SendOrPostCallback d, object? state)
        {
            d(state);
        }

        public void Run(Time time)
        {
            bool workDone = true;

            while (workDone)
            {
                workDone = false;
                while (_timeQueue.TryPeek(out var task, out var priority) && priority <= time)
                {
                    _timeQueue.Dequeue();
                    task();
                    workDone = true;
                }

                while (_workQueue.TryDequeue(out var item))
                {
                    item.Item1(item.Item2);
                    workDone = true;
                }
            }
        }
    }
}