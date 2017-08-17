using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Common.Logging;

namespace Worldpay.Innovation.WPWithin.Sample.Commands
{

    /// <summary>
    /// A very simple producer that offers a single service for charging cars, with a single price.
    /// </summary>
    /// <remarks>This class demonstrates how to set up a simple producer that runs on a separate thread, showing how to run multiple producers and consumers within a single application.</remarks>
    internal class SimpleProducer
    {
        private static readonly ILog Log = LogManager.GetLogger<SimpleProducer>();
        private readonly TextWriter _error;
        private readonly WPWithinService _service;
        private readonly TextWriter _output;
        private Task _task;


        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="output">Where output will be written to.</param>
        /// <param name="error">Where errors will be written to (currently unused).</param>
        /// <param name="service">An initialised service instance.</param>
        public SimpleProducer(TextWriter output, TextWriter error, WPWithinService service)
        {
            _output = output;
            _error = error;
            _service = service;
        }


        /// <summary>
        /// Sets up a single service with a single price offering and then waits 20 seconds for consumers to use the service before exiting.
        /// </summary>
        /// <returns><see cref="CommandResult.Success"/> or throws an exception.</returns>
        public CommandResult Start()
        {
            _output.WriteLine("WorldpayWithin Sample Producer...");

            _service.SetupDevice(".NET Producer Example", $"Example WorldpayWithin producer running on {Dns.GetHostName()}");

            /*
             * Creates a simple electric car charging service, that offers a price to deliver 1 kWh of electricy for £25.
             */
            Service svc = new Service
            {
                Name = "Car charger",
                Description = "Can charge your hybrid / electric car",
                Id = 1,
                Prices = new Dictionary<int, Price>
                {
                    {
                        1, new Price(1) // Note the same price ID must be specified in both the price constructor and the dictionary entry key.
                        {
                            Description = "Kilowatt-hour",
                            UnitDescription = "One kilowatt-hour",
                            UnitId = 1,
                            PricePerUnit = new PricePerUnit(25, "GBP")
                        }
                    }
                }
            };

            _service.AddService(svc);

            /* Initialises the producer (but doesn't start it yet) with the service and client keys for the Worldpay Online Payments service.
             */
            _service.InitProducer("T_C_03eaa1d3-4642-4079-b030-b543ee04b5af", "T_S_f50ecb46-ca82-44a7-9c40-421818af5996");

            Log.Info("Starting service broadcast");

            /* Asynchronously broadcast the service's availablility until stopped
             */
            _task = Task.Run(() => _service.StartServiceBroadcast(0));
            return CommandResult.Success;
        }

        /// <summary>
        /// Assuming that the service is broadcasting (i.e. <see cref="Start"/> has been called), this method stops the broadcast early by telling the service
        /// to stop broadcasting and waiting for the task to complete.
        /// </summary>
        public void Stop()
        {
            _output.WriteLine("Stopping service broadcast");
            _service.StopServiceBroadcast();
            _output.WriteLine("Waiting for producer task to complete");
            _task.Wait();
            _output.WriteLine("Producer task terminated.");
        }
    }
}