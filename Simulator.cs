using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficSim.Processors;

namespace TrafficSim
{
    internal class Simulator
    {
        private readonly ILogger<Simulator> _logger;
        private readonly IMeterFactory _meterFactory;
        private readonly Clock _clock;
        private readonly TaskQueue _taskQueue;
        private readonly int _step = 1;

        public Simulator(ILogger<Simulator> logger, IMeterFactory factory)
        {
            _logger = logger;
            _meterFactory = factory;
            _clock = new Clock();
            _taskQueue = new TaskQueue(_clock);
        }

        public async Task Run()
        {

            var s = new StatusProcessor<string>("Status", 0, _clock, _meterFactory);
            var r = new RateProcessor<string>("Rate", 500, s, _clock, _meterFactory);
            var g = new PeakGenerator("Generator", r, _clock, _meterFactory);
//            var g = new Generator<string>("Generator", 600, r, _clock, _meterFactory);

            for (int j = 0; j < 1800; ++j)
            {
                g.Generate();

                //_taskQueue.Run(_clock.Now);

                await Task.Delay(100);
                _clock.Advance(_step);
            }


        }


/*
     Sync Context reseting

//            //var prevContext = SynchronizationContext.Current;
            //SynchronizationContext.SetSynchronizationContext(_taskQueue);

            //SynchronizationContext.SetSynchronizationContext(prevContext);
 */
    }
}
