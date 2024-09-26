using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSim.Processors
{
    internal class CircuitBreakerProcessor<T> : Processor<T>
    {
        public CircuitBreakerProcessor(string name, IClock clock, IMeterFactory f)
            : base(name, clock, f)
        {
        }

        protected override Task<Result> HandleProcessAsync(T args)
        {
            throw new NotImplementedException();
        }
    }
}
