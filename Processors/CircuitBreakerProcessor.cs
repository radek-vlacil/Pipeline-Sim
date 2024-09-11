using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSim.Processors
{
    internal class CircuitBreakerProcessor<T> : Processor<T>
    {
        public CircuitBreakerProcessor(string name, IClock clock)
            : base(name, clock)
        {
        }

        protected override Task<Result> HandleProcessAsync(T args)
        {
            throw new NotImplementedException();
        }
    }
}
