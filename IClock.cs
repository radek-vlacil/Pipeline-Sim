using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSim
{

    internal interface IClock
    {
        public int Now { get; }

        public void Advance(int duration);
    }
}
