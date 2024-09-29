using System.Diagnostics.Metrics;

namespace TrafficSim.Processors
{
    internal class TrafficShaper : Processor<string>
    {
        private class Counter
        {
            private readonly int _sampleCount;
            private int[] _totals;
            private int[] _priority;
            private int[] _failures;
            private int _currentPriority;
            private int _currentTotal;
            private int _currentFailures;
            private int _index;

            public Counter(int sampleCount)
            {
                _sampleCount = sampleCount;

                _totals = new int[_sampleCount];
                _failures = new int[_sampleCount];
                _priority = new int[_sampleCount];
                _currentTotal = 0;
                _currentFailures = 0;
                _currentPriority = 0;
            }

            public void Update(ItemClassification item, ResultClassification result)
            {
                if (result == ResultClassification.Failure)
                {
                    _failures[_index]++;
                    _currentFailures++;
                }

                _totals[_index]++;
                _currentTotal++;

                if (item == ItemClassification.Priority)
                {
                    _priority[_index]++;
                    _currentPriority++;
                }
            }

            public (int Total, int Failures, int Priority) Get()
            {
                return (_currentTotal, _currentFailures, _currentPriority);
            }
            
            public void Tick()
            {
                _index = (_index + 1) % _sampleCount;
                _currentTotal -= _totals[_index];
                _currentFailures -= _failures[_index];
                _currentPriority -= _priority[_index];
                _totals[_index] = 0;
                _failures[_index] = 0;
                _priority[_index] = 0;
            }
        }

        private enum ItemClassification
        {
            Priority,
            Normal
        }

        private enum ResultClassification
        {
            Success,
            Failure
        }

        private enum InternalState
        {
            Ok,
            Throttling
        }

        private readonly IProcessor<string> _processor;
        private readonly long _failureRate;
        private readonly long _minimum;
        private readonly Counter _counter;
        private int _openPercentage;
        private int _lastStateChange;
        private int _lastTick;

        public TrafficShaper(string name, IProcessor<string> processor, long failureRate, long minimum, IClock clock, IMeterFactory factory)
            : base(name, clock, factory)
        {
            _processor = processor;
            _failureRate = failureRate;
            _minimum = minimum;
            _counter = new Counter(5);
            _openPercentage = 100;
            _lastStateChange = -1;
            _lastTick = 0;
        }

        protected override async Task<Result> HandleProcessAsync(string args)
        {
            Result result = Result.FailureDropped;
            var item = args == "Keep" ? ItemClassification.Priority : ItemClassification.Normal;

            if (CanSend(item))
            {
                result = await _processor.ProcessAsync(args);

                UpdateState(item, result);
            }

            return result;
        }

        private bool CanSend(ItemClassification classification)
        {
            if (classification == ItemClassification.Priority)
            {
                return true;
            }
            else
            {
                if (_openPercentage == 100)
                {
                    return true;
                }
                else
                {
                    return Random.Shared.Next(0, 100) < _openPercentage;
                }
            }
        }
            
        private void UpdateState(ItemClassification item, Result result)
        {
            var resultClassification = result == Result.Throttled ? ResultClassification.Failure : ResultClassification.Success;
            _counter.Update(item, resultClassification);

            var currentTick = Clock.Now;

            UpdateOpenState(currentTick);

            if (currentTick == _lastTick) return;
            _lastTick = currentTick;
            _counter.Tick();
        }

        private void UpdateOpenState(int currentTick)
        {
            if (_lastStateChange == currentTick) return;

            var lastOpenPercentage = _openPercentage;

            var (total, failures, priority) = _counter.Get();
            var rate = (failures * 100) / total;

            UpdateAdditive(rate, total);
            // UpdateHalvingAdditive(rate, total);
            // UpdateFailureRateAdjusting(rate, total, priority);

            if (lastOpenPercentage != _openPercentage)
            {
                _lastStateChange = currentTick;
            }
        }

        private void UpdateAdditive(int failureRate, int total)
        {
            if (failureRate <= 0 || total < _minimum)
            {
                _openPercentage = int.Min(_openPercentage + 2, 100);
            }
            else
            {
                _openPercentage = int.Max(_openPercentage - 2, 1);
            }
        }

        private void UpdateHalvingAdditive(int failureRate, int total)
        {
            if (failureRate < _failureRate || total < _minimum)
            {
                _openPercentage = int.Min(_openPercentage + 10, 100);
            }
            else
            {
                _openPercentage = int.Max(_openPercentage / 2, 1);
            }
        }

        private void UpdateFailureRateAdjusting(int failureRate, int total, int priority)
        {
            if (failureRate > 0)
            {
                _openPercentage = int.Max((_openPercentage + ((100 - failureRate) - (failureRate * priority) / (total - priority) - 5)) / 2, 1);
            }
            else
            {
                _openPercentage = int.Min(_openPercentage + 5, 100);
            }
        }
    }
}
