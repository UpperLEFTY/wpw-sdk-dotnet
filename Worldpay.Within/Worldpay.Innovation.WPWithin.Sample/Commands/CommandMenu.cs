using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Worldpay.Innovation.WPWithin.AgentManager;

namespace Worldpay.Innovation.WPWithin.Sample.Commands
{

    /// <summary>
    /// The main logic of the sample application.  Contains the menu items and the functions that are executed when a menu item is selected.
    /// </summary>
    internal class CommandMenu
    {
        private readonly TextWriter _error;

        private readonly List<Command> _menuItems;
        private readonly TextWriter _output;
        private readonly TextReader _reader;
        private RpcAgentManager _rpcManager;
        private SimpleProducer _simpleProducer;
        private readonly RpcAgentConfiguration _defaultAgentConfig;
        private WPWithinService _service;

        public CommandMenu()
        {
            _menuItems = new List<Command>(new[]
            {
                new Command("Exit", "Exits the application.", (a) =>
                {
                    _output.WriteLine("Exiting");
                    return CommandResult.Exit;
                }),
                new Command("StartRPCClient", "Starts a default Thrift RPC agent", StartRpcClient),
                new Command("StopRPCClient", "Stops the default Thrift RPC agent", StopRpcClient),
                new Command("StartSimpleProducer", "Starts a simple producer", StartSimpleProducer),
                new Command("StopSimpleProducer", "Starts a simple producer", StopSimpleProducer),
                new Command("ConsumePurchase", "Consumes a service (first price of first service found)", ConsumePurchase),
                new Command("FindProducers", "Lists out all the producers that could be found", FindProducers),
            });

            // TODO Parameterise these so output can be written to a specific file
            _output = Console.Out;
            _error = Console.Error;
            _reader = Console.In;
            _defaultAgentConfig = new RpcAgentConfiguration
            {
                ServicePort = 9091,
                CallbackPort = 9092,
                LogLevel = "panic,fatal,error,warn,info,debug",
                LogFile = new FileInfo("wpwithin.log")
            };
        }

        private CommandResult FindProducers(string[] arg)
        {
            RpcAgentConfiguration consumerConfig = new RpcAgentConfiguration
            {
                LogLevel = "panic,fatal,error,warn,info,debug",
                LogFile = new FileInfo("WPWithinConsumer.log"),
                ServicePort = 9096,
            };
            RpcAgentManager consumerAgent = new RpcAgentManager(consumerConfig);
            consumerAgent.StartThriftRpcAgentProcess();
            try
            {
                WPWithinService service = new WPWithinService(consumerConfig);
                service.SetupDevice("Scanner", $".NET Sample Producer scanner running on ${Dns.GetHostName()}");
                List<ServiceMessage> devices = service.DeviceDiscovery(10000).ToList();

                _output.WriteLine("Found total of {0} devices", devices.Count);
                for (int deviceIndex = 0; deviceIndex < devices.Count; deviceIndex++)
                {
                    ServiceMessage device = devices[deviceIndex];
                    _output.WriteLine(
                        $"Device {deviceIndex}) {device.ServerId} running on {device.Hostname}:{device.PortNumber}");
                    _output.WriteLine($"\tDescription: {device.DeviceDescription}, URL Prefix: {device.UrlPrefix}");
                    service.InitConsumer("http://", device.Hostname, device.PortNumber ?? 80, device.UrlPrefix,
                        device.ServerId,
                        new HceCard("Bilbo", "Baggins", "Card", "5555555555554444", 11, 2018, "113"));
                    try
                    {
                        List<ServiceDetails> services = service.RequestServices().ToList();
                        _output.WriteLine("\t{0} services found on device {1}", services.Count, deviceIndex);
                        for (int serviceIndex = 0; serviceIndex < services.Count; serviceIndex++)
                        {
                            ServiceDetails svc = services[serviceIndex];
                            _output.WriteLine($"\t\tService {serviceIndex}) {svc.ServiceId}: {svc.ServiceDescription}");
                            List<Price> prices = service.GetServicePrices(svc.ServiceId).ToList();
                            foreach (Price price in prices)
                            {
                                _output.WriteLine("\t\t\tPrice: {0}", price);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _error.WriteLine(ex);
                    }
                }
            }
            finally
            {
                consumerAgent.StopThriftRpcAgentProcess();
            }
            return CommandResult.Success;

        }

        private CommandResult ConsumePurchase(string[] arg)
        {
            RpcAgentConfiguration consumerConfig = new RpcAgentConfiguration
            {
                LogLevel = "panic,fatal,error,warn,info,debug",
                LogFile = new FileInfo("WPWithinConsumer.log"),
                ServicePort = 9096,
            };
            RpcAgentManager consumerAgent = new RpcAgentManager(consumerConfig);
            consumerAgent.StartThriftRpcAgentProcess();

            WPWithinService service = new WPWithinService(consumerConfig);
            SimpleConsumer consumer = new SimpleConsumer(_output, _error);
            consumer.MakePurchase(service);

            consumerAgent.StopThriftRpcAgentProcess();
            return CommandResult.Success;
        }

        private CommandResult StartSimpleProducer(string[] arg)
        {
            if (_simpleProducer != null)
            {
                _output.WriteLine("Simple producer already started, stop it before trying to start it again.");
                return CommandResult.NonCriticalError;
            }
            _simpleProducer = new SimpleProducer(_output, _error, _service);
            _simpleProducer.Start();
            return CommandResult.Success;
        }

        private CommandResult StopSimpleProducer(string[] arg)
        {
            if (_simpleProducer == null)
            {
                _output.WriteLine("Cannot stop Simple producer as it is not started.");
                return CommandResult.NonCriticalError;
            }
            _simpleProducer.Stop();
            _simpleProducer = null;
            return CommandResult.Success;
        }

        private CommandResult StopRpcClient(string[] arg)
        {
            if (_rpcManager == null)
            {
                _error.WriteLine("Thift RPC Agent not active.  Start it before trying to stop it.");
                return CommandResult.NonCriticalError;
            }
            _service.Dispose();
            _service = null;
            _rpcManager.StopThriftRpcAgentProcess();
            _rpcManager = null;
            return CommandResult.Success;
        }

        private CommandResult StartRpcClient(string[] arg)
        {
            if (_rpcManager != null)
            {
                _error.WriteLine("Thrift RPC Agent already active.  Stop it before trying to start a new one");
                return CommandResult.NonCriticalError;
            }
            _rpcManager = new RpcAgentManager(new RpcAgentConfiguration());
            _rpcManager.StartThriftRpcAgentProcess();
            _service = new WPWithinService(_defaultAgentConfig);
            return CommandResult.Success;
        }

        internal CommandResult ReadEvalPrint(string[] args)
        {
            // Only show the menu if there isn't already a command line to deal with
            if (args != null)
            {
                _output.WriteLine("\nSample Application.");
                int count = 0;
                foreach (Command item in _menuItems)
                {
                    _output.WriteLine("{0}. {1}: {2}", count, item.Name, item.Description);
                    count++;
                }

                // Read
                _output.Write("\nCommand: ");
                string readLine = _reader.ReadLine();
                if (readLine == null)
                {
                    return CommandResult.NoOp;
                }

                args = readLine.Split();
            }

            // If no command was entered, then simply return a no-op response.
            if (args == null || args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                return CommandResult.NoOp;
            }

            int optionNumber;
            // We accept either specifying a command by number or by name.
            Command selectedItem = int.TryParse(args[0], out optionNumber) ? _menuItems[optionNumber] : _menuItems.FirstOrDefault(m => m.Name.Equals(args[0]));

            if (selectedItem != null)
            {
                try
                {
                    return selectedItem.Function(args);
                }
                catch (WPWithinException wpwe)
                {
                    _error.WriteLine(wpwe);
                    return CommandResult.NonCriticalError;
                }
                catch (Exception wpwe)
                {
                    _error.WriteLine(wpwe);
                    return CommandResult.CriticalError;
                }
            }

            _output.WriteLine($"Invalid command: \"{args[0]}\"");
            return CommandResult.NoSuchCommand;
        }
    }
}