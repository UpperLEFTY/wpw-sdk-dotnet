using System;
using System.Collections.Generic;
using Common.Logging;
using Thrift.Protocol;
using Thrift.Transport;
using Worldpay.Innovation.WPWithin.AgentManager;
using Worldpay.Innovation.WPWithin.EventListener;
using Worldpay.Innovation.WPWithin.ThriftAdapters;
using ThriftWPWithinService = Worldpay.Innovation.WPWithin.Rpc.WPWithin;


namespace Worldpay.Innovation.WPWithin
{
    /// <summary>
    ///     The main Worldpay Within service endpoint.
    /// </summary>
    /// <remarks>
    ///     <para>To access a Worldpay Within service create an instance of this class, configured with a <see cref="RpcAgentConfiguration"/> instance that informs this instance
    /// how to connect to the Thrift RPC Agent that manages the communication to remote devices.</para>
    /// <para>Note that this single service interface is the used for both producing (selling) and consuming (buying) services.</para>
    /// </remarks>
    public class WPWithinService : IDisposable
    {
        /// <summary>
        /// Delegate used for registering handlers for the <see cref="WPWithinService.OnBeginServiceDelivery"/> event.
        /// </summary>
        /// <param name="serviceId">The identity of the service that has begun delivering its service.</param>
        /// <param name="serviceDeliveryToken">A token that can be validated to ensure that origin of the event is genuine.</param>
        /// <param name="unitsToSupply">The number of units that will be supplied.</param>
        public delegate void BeginServiceDeliveryHandler(
            int serviceId, ServiceDeliveryToken serviceDeliveryToken, int unitsToSupply);

        /// <summary>
        /// Delegate used for registering handlers for the <see cref="WPWithinService.OnEndServiceDelivery"/> event.
        /// </summary>
        /// <param name="serviceId">The identity of the service that has completed delivering its service.</param>
        /// <param name="serviceDeliveryToken">A token that can be validated to ensure that origin of the event is genuine.</param>
        /// <param name="unitsReceived">The number of units that have been supplied.</param>
        public delegate void EndServiceDeliveryHandler(
            int serviceId, ServiceDeliveryToken serviceDeliveryToken, int unitsReceived);

        private static readonly ILog Log = LogManager.GetLogger<WPWithinService>();

        private readonly CallbackServerManager _listener;

        private ThriftWPWithinService.Client _client;
        private bool _isDisposed;
        private TTransport _transport;

        /// <summary>
        /// Creates an instance of the service that will communicate with remote devices using the configuration supplied by <paramref name="localAgentConfiguration"/>.
        /// </summary>
        /// <param name="localAgentConfiguration">Describes how the local Thrift RPC agent (the core SDK) can be communicated with.</param>
        public WPWithinService(RpcAgentConfiguration localAgentConfiguration)
        {
            if (localAgentConfiguration == null)
                throw new ArgumentNullException("A configuration must be supplied", nameof(localAgentConfiguration));
            InitClient(localAgentConfiguration);
            if (localAgentConfiguration.CallbackPort > 0)
            {
                _listener = new CallbackServerManager(localAgentConfiguration);
                _listener.Start();
            }
            else
            {
                Log.Info("Callbacks disabled as port specified as 0");
            }
        }


        /// <summary>
        ///     Convenience constructor for minimal configuration with no callbacks using defaults wherever possible.
        /// </summary>
        /// <param name="host">The host to find/create the RPC agent on.</param>
        /// <param name="port">The port to find/create the RPC agent on.</param>
        public WPWithinService(string host, int port) : this(host, port, 0)
        {
        }

        /// <summary>
        ///     Convenience constructor for minimal configuration with callbacks using defaults wherever possible.
        /// </summary>
        /// <param name="host">The host to find/create the RPC agent on.</param>
        /// <param name="port">The port to find/create the RPC agent on.</param>
        /// <param name="callbackPort">The callback port to listen on (if 0 then no callbacks will be enabled).</param>
        public WPWithinService(string host, int port, int callbackPort) : this(new RpcAgentConfiguration
        {
            ServiceHost = host,
            ServicePort = port,
            CallbackPort = callbackPort
        })
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Event raised for consumers when a service that has been paid for starts delivering that service.
        /// </summary>
        /// <remarks>This event will only be raised if <see cref="RpcAgentConfiguration.CallbackPort"/> has been set, which causes the Thrift RPC Agent to set up a callback
        /// mechanism.</remarks>
        public event BeginServiceDeliveryHandler OnBeginServiceDelivery
        {
            add { _listener.BeginServiceDelivery += value; }
            remove { _listener.BeginServiceDelivery -= value; }
        }

        /// <summary>
        /// Event raised for consumers when a service that has been paid for completes delivering that service.
        /// </summary>
        /// <remarks>This event will only be raised if <see cref="RpcAgentConfiguration.CallbackPort"/> has been set, which causes the Thrift RPC Agent to set up a callback
        /// mechanism.</remarks>
        public event EndServiceDeliveryHandler OnEndServiceDelivery
        {
            add { _listener.EndServiceDelivery += value; }
            remove { _listener.EndServiceDelivery -= value; }
        }

        /// <summary>
        /// Adds a new service to those offered by this producer.
        /// </summary>
        /// <param name="service">The service to add.  Must not be null.</param>
        public void AddService(Service service)
        {
            _client.addService(ServiceAdapter.Create(service));
        }

        /// <summary>
        /// Removes a service, so that it is no longer offered by this producer.
        /// </summary>
        /// <param name="service">The service to remove.</param>
        public void RemoveService(Service service)
        {
            _client.removeService(ServiceAdapter.Create(service));
        }


        public void InitConsumer(string scheme, string hostname, int port, string urlPrefix, string serviceId,
            HceCard hceCard)
        {
            _client.initConsumer(scheme, hostname, port, urlPrefix, serviceId, HceCardAdapter.Create(hceCard));
        }

        public void InitProducer(string merchantClientKey, string merchantServiceKey)
        {
            _client.initProducer(merchantClientKey, merchantServiceKey);
        }

        public Device GetDevice()
        {
            return DeviceAdapter.Create(_client.getDevice());
        }

        public void StopServiceBroadcast()
        {
            _client.stopServiceBroadcast();
        }

        public IEnumerable<ServiceMessage> DeviceDiscovery(int timeoutMillis)
        {
            return ServiceMessageAdapter.Create(_client.deviceDiscovery(timeoutMillis));
        }

        public IEnumerable<ServiceDetails> RequestServices()
        {
            return ServiceDetailsAdapter.Create(_client.requestServices());
        }

        public IEnumerable<Price> GetServicePrices(int serviceId)
        {
            return PriceAdapter.Create(_client.getServicePrices(serviceId));
        }

        public TotalPriceResponse SelectService(int serviceId, int numberOfUnits, int priceId)
        {
            return TotalPriceResponseAdapter.Create(_client.selectService(serviceId, numberOfUnits, priceId));
        }

        public PaymentResponse MakePayment(TotalPriceResponse request)
        {
            return PaymentResponseAdapter.Create(_client.makePayment(TotalPriceResponseAdapter.Create(request)));
        }

        public void BeginServiceDelivery(int serviceId, ServiceDeliveryToken serviceDeliveryToken, int unitsToSupply)
        {
            _client.beginServiceDelivery(serviceId, ServiceDeliveryTokenAdapter.Create(serviceDeliveryToken),
                unitsToSupply);
        }

        public void EndServiceDelivery(int serviceId, ServiceDeliveryToken serviceDeliveryToken, int unitsReceived)
        {
            _client.endServiceDelivery(serviceId, ServiceDeliveryTokenAdapter.Create(serviceDeliveryToken),
                unitsReceived);
        }

        public void StartServiceBroadcast(int timeoutMillis)
        {
            _client.startServiceBroadcast(timeoutMillis);
        }

        public void SetupDevice(string deviceName, string deviceDescription)
        {
            _client.setup(deviceName, deviceDescription);
        }

        private void InitClient(RpcAgentConfiguration config)
        {
            TTransport transport = config.GetThriftTransport();
            transport.Open();
            TProtocol protocol = config.GetThriftProtcol(transport);
            ThriftWPWithinService.Client client = new ThriftWPWithinService.Client(protocol);

            _transport = transport;
            _client = client;
            Log.InfoFormat("Client connected to Thrift RPC Agent endpoint at {0}:{1} using {2}", config.ServiceHost,
                config.ServicePort, config.Protocol);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
            try
            {
                _transport.Close();
            }
            catch (Exception e)
            {
                Log.Warn("Error closing connection to RPC Agent", e);
            }

            if (_listener != null)
            {
                try
                {
                    _listener.Stop();
                }
                catch (Exception e)
                {
                    Log.Warn("Error stopping callback listener", e);
                }
            }
            //Dispose of resources here
            _isDisposed = true;
        }

        ~WPWithinService()
        {
            Dispose(false);
        }
    }
}