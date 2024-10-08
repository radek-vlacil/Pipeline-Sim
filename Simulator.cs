﻿using Microsoft.Extensions.Logging;
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
        private readonly PeriodicTimer _periodicTimer;

        public Simulator(ILogger<Simulator> logger, IMeterFactory factory)
        {
            _logger = logger;
            _meterFactory = factory;
            _clock = new Clock();
            _taskQueue = new TaskQueue(_clock);
            _periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        }

        public async Task Run()
        {

            var status = new StatusProcessor<string>("Stats", 0, _clock, _meterFactory);
            var rate   = new RateProcessor<string>("Rate", 5000, status, _clock, _meterFactory);
            var shaper = new TrafficShaper("Shaper", rate, 10, 100, _clock, _meterFactory);
            var retry  = new RetryProcessor<string>("Retry", shaper, 3, 100, ((i, j) => i == "Keep" && j != Result.Success), _clock, _taskQueue, _meterFactory);
            var g      = new PeakGenerator("Generator", retry, _clock, _meterFactory);
//            var g = new Generator<string>("Generator", 600, shaper, _clock, _meterFactory);

            int i = 0;
            const int maxPeriods = 1800;
            while (await _periodicTimer.WaitForNextTickAsync())
            {
                if (i++ == maxPeriods) return;
                g.Generate("Drop");

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
