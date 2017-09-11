﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Common.Logging;

using Worldpay.Within.ThriftAdapters;
using Worldpay.Within.AgentManager;
using System.Threading;

namespace Worldpay.Within.Sample.Commands
{

    /// <summary>
    /// A very simple producer that offers a single service for charging cars, with a single price.
    /// </summary>
    /// <remarks>This class demonstrates how to set up a simple producer that runs on a separate thread, showing how to run multiple producers and consumers within a single application.</remarks>
    internal class SimpleProducer
    {
        private static readonly ILog Log = LogManager.GetLogger<SimpleProducer>();
        private readonly TextWriter _error;
        private WPWithinService _service;
        private readonly TextWriter _output;
        private Task _task;
        private RpcAgentManager _rpcManager;

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="output">Where output will be written to.</param>
        /// <param name="error">Where errors will be written to (currently unused).</param>
        /// <param name="service">An initialised service instance.</param>
        public SimpleProducer(TextWriter output, TextWriter error, WPWithinService service, RpcAgentManager rpcAgent)
        {
            _output = output;
            _error = error;
            _service = service;
            _rpcManager = rpcAgent;
        }


        /// <summary>
        /// Sets up a single service with a single price offering and then waits 20 seconds for consumers to use the service before exiting.
        /// </summary>
        /// <returns><see cref="CommandResult.Success"/> or throws an exception.</returns>
        public CommandResult Start()
        {
            _output.WriteLine("WorldpayWithin Sample Producer...");
            _service.SetupDevice(".NET Producer Example", $"Example WorldpayWithin producer running on {Dns.GetHostName()}");
            _rpcManager?.StartThriftRpcAgentProcess();
            Thread.Sleep(750);
            /*
             * Creates a simple electric car charging service, that offers a price to deliver 1 kWh of electricy for £25.
             */
            Service svc = new Service(1)
            {
                Name = "Car charger",
                Description = "Can charge your hybrid / electric car",
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
            PspConfig config = new PspConfig
            {
                HtePublicKey = "T_C_03eaa1d3-4642-4079-b030-b543ee04b5af",
                HtePrivateKey = "T_S_f50ecb46-ca82-44a7-9c40-421818af5996",
                MerchantClientKey = "T_C_03eaa1d3-4642-4079-b030-b543ee04b5af",
                MerchantServiceKey = "T_S_f50ecb46-ca82-44a7-9c40-421818af5996"
            };
            _service.InitProducer(config);

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
            _service?.StopServiceBroadcast();
            _output.WriteLine("Waiting for producer task to complete, max 250ms");
            _task.Wait(250);
            try
            {
                _service?.CloseRPCAgent();
            }
            catch { }
            _service = null;
            _output.WriteLine("Producer task terminated.");
        }

        /// <summary>
        /// Starts the RPC Agent client process.
        /// </summary>
        private void StartRpcClient()
        {
            if (_rpcManager != null)
            {
                _error.WriteLine("Thrift RPC Agent already active.  Stop it before trying to start a new one");
                return;
            }
            _rpcManager = new RpcAgentManager(new RpcAgentConfiguration
            {
                LogLevel = RpcAgentConfiguration.LogLevelAll
            });
            _rpcManager.StartThriftRpcAgentProcess();

            return;
        }

        /// <summary>
        /// Stops the RPC Agent client process. CloseRPCAgent() initiates os.Exit() on RPC agent.
        /// </summary>
        internal void StopRpcClient()
        {
            if (_rpcManager == null)
            {
                _error.WriteLine("Thift RPC Agent not active.  Start it before trying to stop it.");
                return;
            }
            try
            {
                _service?.CloseRPCAgent();
            }
            catch { }
            _service?.Dispose();
            _service = null;
            _rpcManager?.StopThriftRpcAgentProcess();
            _rpcManager = null;
            return;
        }
    }
}