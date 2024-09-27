using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.Metrics;

namespace TrafficSim.Processors
{
    internal class TrafficShaper : Processor<string>
    {
        private class Counter
        {
            private readonly int _sampleCount;
            private int[] _totals;
            private int[] _failures;
            private int _currentTotal;
            private int _currentFailures;
            private int _index;

            public Counter(int sampleCount)
            {
                _sampleCount = sampleCount;

                _totals = new int[_sampleCount];
                _failures = new int[_sampleCount];
                _currentTotal = 0;
                _currentFailures = 0;
            }

            public void Update(ResultClassification result)
            {

                if (result == ResultClassification.Failure)
                {
                    _failures[_index]++;
                    _currentFailures++;
                }

                _totals[_index]++;
                _currentTotal++;
            }

            public (int Total, int Failures) Get()
            {
                return (_currentTotal, _currentFailures);
            }
            
            public void Tick()
            {
                _index = (_index + 1) % _sampleCount;
                _currentTotal -= _totals[_index];
                _currentFailures -= _failures[_index];
                _totals[_index] = 0;
                _failures[_index] = 0;
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
            if (CanSend(args))
            {
                result = await _processor.ProcessAsync(args);

                UpdateState(result);
            }

            return result;
        }

        private bool CanSend(string requestType)
        {
            var classification = requestType == "Keep" ? ItemClassification.Priority : ItemClassification.Normal;

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
            
        private void UpdateState(Result result)
        {
            var resultClassification = result == Result.Throttled ? ResultClassification.Failure : ResultClassification.Success;
            _counter.Update(resultClassification);

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

            var (total, failures) = _counter.Get();
            var rate = (failures * 100) / total;

            if (rate < _failureRate || total < _minimum)
            {
                _openPercentage = int.Min(_openPercentage + 10, 100);
            }
            else
            {
                _openPercentage = int.Max(_openPercentage / 2, 1);
            }

            if (lastOpenPercentage != _openPercentage)
            {
                _lastStateChange = currentTick;
            }
        }
    }
}
