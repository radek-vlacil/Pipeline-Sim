using TrafficSim.Processors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TrafficSim
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseMetricServer();


            app.MapGet("/run", async (ILogger<Simulator> log, IMeterFactory f) =>
            {
                var sim = new Simulator(log, f);
                var _ = sim.Run();
                return Results.Ok();
            });

            app.Run();



 //           var s1 = new StatusProcessor("Primary.Status", [(0, 0), (20000, 100), (400000, 0)], clock);
 //           var d1 = new DelayProcessor("Primary.Delay", [(0, 150), (20000, 1000), (400000, 150)], s1, clock, taskQueue);
 //           var t1 = new TimeoutProcessor("Primary.TimeoutL", 500, d1, clock, taskQueue);
 //           var cb1 = new BulkheadProcessor("Primary.Bulkhead", 500, 100, t1, clock);
 //           var to1 = new TimeoutProcessor("Primary.TimeoutO", 1000, t1, clock, taskQueue);

 //           var s2 = new StatusProcessor("Secondary.Status", Result.Success, clock);
 //           var d2 = new DelayProcessor("Secondary.Delay", [(0, 100)], s2, clock, taskQueue);
 //           var t2 = new TimeoutProcessor("Secondary.TimeoutL", 500, d2, clock, taskQueue);
 //           var cb2 = new BulkheadProcessor("Secondary.Bulkhead", 500, 10000, t2, clock);
 //           var to2 = new TimeoutProcessor("Secondary.TimeoutO", 1000, t2, clock, taskQueue);

 //           var h = new HedgingProcessor("Hedging", 500, to1, to2, clock, taskQueue);

            /*
            var s = new StatusProcessor<string>("Status", 0, clock);
            var r = new RateProcessor<string>("Rate", 1, s, clock);
            var g = new PeakGenerator("Generator", 1, r, clock);

            var processors = new List<IProcessor> { g };
            var p1 = new SingleFilePrinter(processors);
            //var p2 = new MultiFilePrinter(processors);
            p1.PrintTile();
            //p2.PrintTile();

            for (int j = 0; j < 1800; ++j)
            {
                g.Generate();
                taskQueue.Run(clock.Now);
                p1.Print();

                clock.Advance(step);
            }
            */
        }
    }
}
