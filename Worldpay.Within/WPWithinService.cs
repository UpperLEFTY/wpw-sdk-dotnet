using System;
using System.Collections.Generic;
using Common.Logging;
using Thrift;
using Thrift.Protocol;
using Thrift.Transport;
using Worldpay.Within.AgentManager;
using Worldpay.Within.EventListener;
using Worldpay.Within.ThriftAdapters;
using ThriftWPWithinService = Worldpay.Within.Rpc.WPWithin;


namespace Worldpay.Within
{
    /// <summary>
    ///     The main Worldpay Within service endpoint.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         To access a Worldpay Within service create an instance of this class, configured with a
    ///         <see cref="RpcAgentConfiguration" /> instance that informs this instance
    ///         how to connect to the Thrift RPC Agent that manages the communication to remote devices.
    ///     </para>
    ///     <para>
    ///         Note that this single service interface is the used for both producing (selling) and consuming (buying)
    ///         services.
    ///     </para>
    /// </remarks>
    public class WPWithinService : IDisposable
    {
        /// <summary>
        ///     Delegate used for registering handlers for the <see cref="WPWithinService.OnBeginServiceDelivery" /> event.
        /// </summary>
        /// <param name="serviceId">The identity of the service that has begun delivering its service.</param>
        /// <param name="servicePriceID">The identity of the service price that has begun delivering.</param>
        /// <param name="serviceDeliveryToken">A token that can be validated to ensure that origin of the event is genuine.</param>
        /// <param name="unitsToSupply">The number of units that will be supplied.</param>
        public delegate void BeginServiceDeliveryHandler(
            int serviceId, int servicePriceID, ServiceDeliveryToken serviceDeliveryToken, int unitsToSupply);

        /// <summary>
        ///     Delegate used for registering handlers for the <see cref="WPWithinService.OnEndServiceDelivery" /> event.
        /// </summary>
        /// <param name="serviceId">The identity of the service that has completed delivering its service.</param>
        /// <param name="serviceDeliveryToken">A token that can be validated to ensure that origin of the event is genuine.</param>
        /// <param name="unitsReceived">The number of units that have been supplied.</param>
        public delegate void EndServiceDeliveryHandler(
            int serviceId, ServiceDeliveryToken serviceDeliveryToken, int unitsReceived);

        /// <summary>
        /// Delegate used for registering handlers for the <see cref="WPWithinService.OnMakePaymentEvent" /> event.
        /// </summary>
        /// <param name="totalPrice">Total price.</param>
        /// <param name="orderCurrency">Currency of the order.</param>
        /// <param name="clientToken">Client token.</param>
        /// <param name="orderDescription">Description of the order.</param>
        /// <param name="uuid">UUID of client.</param>
        public delegate void MakePaymentEventHandler(
            int totalPrice, string orderCurrency, string clientToken, string orderDescription, string uuid);

        /// <summary>
        /// Delegate used for registering handlers for the <see cref="WPWithinService.OnServiceDiscovery" /> event.
        /// </summary>
        /// <param name="remoteAddr">Remote address of consumer.</param>
        public delegate void ServiceDiscoveryEventHandler(string remoteAddr);

        /// <summary>
        /// Delegate used for registering handlers for the <see cref="WPWithinService.OnServicePrices" /> event.
        /// </summary>
        /// <param name="remoteAddr">Remote address of consumer.</param>
        /// <param name="serviceId">The identity of the service that has completed delivering its service.</param>
        public delegate void ServicePricesEventHandler(string remoteAddr, int serviceId);

        /// <summary>
        /// Delegate used for registering handlers for the <see cref="WPWithinService.OnServiceTotalPriceEvent" /> event.
        /// </summary>
        /// <param name="remoteAddr">Remote address of consumer.</param>
        /// <param name="serviceID">The identity of the service that has completed delivering its service.</param>
        /// <param name="totalPriceResp"></param>
        public delegate void ServiceTotalPriceEventHandler(string remoteAddr, int serviceID, Within.TotalPriceResponse totalPriceResp);

        /// <summary>
        /// Delegate used for registering handlers for the <see cref="WPWithinService.OnErrorEvent" /> event.
        /// </summary>
        /// <param name="msg">Error message.</param>
        public delegate void ErrorEventHandler(string msg);


        private static readonly ILog Log = LogManager.GetLogger<WPWithinService>();

        private readonly CallbackServerManager _listener;

        private ThriftWPWithinService.Client _client;
        private bool _isDisposed;
        private TTransport _transport;

        /// <summary>
        ///     Creates an instance of the service that will communicate with remote devices using the configuration supplied by
        ///     <paramref name="localAgentConfiguration" />.
        /// </summary>
        /// <param name="localAgentConfiguration">Describes how the local Thrift RPC agent (the core SDK) can be communicated with.</param>
        public WPWithinService(RpcAgentConfiguration localAgentConfiguration)
        {
            if (localAgentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(localAgentConfiguration), "A configuration must be supplied");
            }
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
        ///     Event raised for consumers when a service that has been paid for starts delivering that service.
        /// </summary>
        /// <remarks>
        ///     This event will only be raised if <see cref="RpcAgentConfiguration.CallbackPort" /> has been set, which causes the
        ///     Thrift RPC Agent to set up a callback
        ///     mechanism.
        /// </remarks>
        public event BeginServiceDeliveryHandler OnBeginServiceDelivery
        {
            add { _listener.BeginServiceDelivery += value; }
            remove { _listener.BeginServiceDelivery -= value; }
        }

        /// <summary>
        ///     Event raised for consumers when a service that has been paid for completes delivering that service.
        /// </summary>
        /// <remarks>
        ///     This event will only be raised if <see cref="RpcAgentConfiguration.CallbackPort" /> has been set, which causes the
        ///     Thrift RPC Agent to set up a callback
        ///     mechanism.
        /// </remarks>
        public event EndServiceDeliveryHandler OnEndServiceDelivery
        {
            add { _listener.EndServiceDelivery += value; }
            remove { _listener.EndServiceDelivery -= value; }
        }

        /// <summary>
        ///     Event raised (for producer) when MakePayment is issued.
        /// </summary>
        /// <remarks>
        ///     This event will only be raised if <see cref="RpcAgentConfiguration.CallbackPort" /> has been set, which causes the
        ///     Thrift RPC Agent to set up a callback
        ///     mechanism.
        /// </remarks>
        public event MakePaymentEventHandler OnMakePaymentEvent
        {
            add { _listener.MakePaymentEvent += value; }
            remove { _listener.MakePaymentEvent -= value; }
        }

        /// <summary>
        ///     Event raised (for producer) when ServiceDiscovery is issued.
        /// </summary>
        /// <remarks>
        ///     This event will only be raised if <see cref="RpcAgentConfiguration.CallbackPort" /> has been set, which causes the
        ///     Thrift RPC Agent to set up a callback
        ///     mechanism.
        /// </remarks>
        public event ServiceDiscoveryEventHandler ServiceDiscoveryEvent
        {
            add { _listener.ServiceDiscoveryEvent += value; }
            remove { _listener.ServiceDiscoveryEvent -= value; }
        }

        /// <summary>
        ///     Event raised (for producer) when ServicePrices is issued.
        /// </summary>
        /// <remarks>
        ///     This event will only be raised if <see cref="RpcAgentConfiguration.CallbackPort" /> has been set, which causes the
        ///     Thrift RPC Agent to set up a callback
        ///     mechanism.
        /// </remarks>
        public event ServicePricesEventHandler OnServicePricesEvent
        {
            add { _listener.ServicePricesEvent += value; }
            remove { _listener.ServicePricesEvent -= value; }
        }

        /// <summary>
        ///      Event raised (for producer) when ServiceTotalPrice is issued.
        /// </summary>
        /// <remarks>
        ///     This event will only be raised if <see cref="RpcAgentConfiguration.CallbackPort" /> has been set, which causes the
        ///     Thrift RPC Agent to set up a callback
        ///     mechanism.
        /// </remarks>
        public event ServiceTotalPriceEventHandler OnServiceTotalPriceEvent
        {
            add { _listener.ServiceTotalPriceEvent += value; }
            remove { _listener.ServiceTotalPriceEvent -= value; }
        }

        /// <summary>
        ///     Event raised when Error is issued from SDK.
        /// </summary>
        /// <remarks>
        ///     This event will only be raised if <see cref="RpcAgentConfiguration.CallbackPort" /> has been set, which causes the
        ///     Thrift RPC Agent to set up a callback
        ///     mechanism.
        /// </remarks>
        public event ErrorEventHandler OnErrorEvent
        {
            add { _listener.ErrorEvent += value; }
            remove { _listener.ErrorEvent -= value; }
        }


        /// <summary>
        ///     Adds a new service to those offered by this producer.
        /// </summary>
        /// <param name="service">The service to add.  Must not be null.</param>
        public void AddService(Service service)
        {
            _client.addService(ServiceAdapter.Create(service));
        }

        /// <summary>
        ///     Removes a service, so that it is no longer offered by this producer.
        /// </summary>
        /// <param name="service">The service to remove.</param>
        public void RemoveService(Service service)
        {
            _client.removeService(ServiceAdapter.Create(service));
        }


        public void InitConsumer(string scheme, string hostname, int port, string urlPrefix, string serviceId,
            HceCard hceCard, PspConfig pspConfig)
        {
            _client.initConsumer(scheme, hostname, port, urlPrefix, serviceId, HceCardAdapter.Create(hceCard),
                pspConfig.ToThriftRepresentation());
        }

        public void InitProducer(PspConfig pspConfig)
        {
            try
            {
                Dictionary<string, string> pspConfigMap = pspConfig?.ToThriftRepresentation();
                try
                {
                    _client.initProducer(pspConfigMap);
                }
                catch (TApplicationException tae)
                {
                    throw new WPWithinException(tae);
                }
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public Device GetDevice()
        {
            try
            {
                return DeviceAdapter.Create(_client.getDevice());
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public void StopServiceBroadcast()
        {
            try
            {
                _client.stopServiceBroadcast();
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public IEnumerable<ServiceMessage> DeviceDiscovery(int timeoutMillis)
        {
            try
            {
                return ServiceMessageAdapter.Create(_client.deviceDiscovery(timeoutMillis));
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public ServiceMessage SearchForDevice(int timeoutMillis, string deviceName)
        {
            try
            {
                return ServiceMessageAdapter.Create(_client.searchForDevice(timeoutMillis, deviceName));
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public IEnumerable<ServiceDetails> RequestServices()
        {
            try
            {
                return ServiceDetailsAdapter.Create(_client.requestServices());
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public IEnumerable<Price> GetServicePrices(int serviceId)
        {
            try
            {
                return PriceAdapter.Create(_client.getServicePrices(serviceId));
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public TotalPriceResponse SelectService(int serviceId, int numberOfUnits, int priceId)
        {
            try
            {
                return TotalPriceResponseAdapter.Create(_client.selectService(serviceId, numberOfUnits, priceId));
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public PaymentResponse MakePayment(TotalPriceResponse request)
        {
            try
            {
                return PaymentResponseAdapter.Create(_client.makePayment(TotalPriceResponseAdapter.Create(request)));
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public void BeginServiceDelivery(int serviceId, ServiceDeliveryToken serviceDeliveryToken, int unitsToSupply)
        {
            try
            {
                _client.beginServiceDelivery(serviceId, ServiceDeliveryTokenAdapter.Create(serviceDeliveryToken),
                    unitsToSupply);
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public void EndServiceDelivery(int serviceId, ServiceDeliveryToken serviceDeliveryToken, int unitsReceived)
        {
            try
            {
                _client.endServiceDelivery(serviceId, ServiceDeliveryTokenAdapter.Create(serviceDeliveryToken),
                    unitsReceived);
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public void StartServiceBroadcast(int timeoutMillis)
        {
            try
            {
                _client.startServiceBroadcast(timeoutMillis);
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public void SetupDevice(string deviceName, string deviceDescription)
        {
            try
            {
                _client.setup(deviceName, deviceDescription);
            }
            catch (TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
        }

        public void CloseRPCAgent()
        {
            try
            {
                _client.CloseRPCAgent();
            }
            catch(TApplicationException tae)
            {
                throw new WPWithinException(tae);
            }
            catch(Exception e)
            {
                throw new WPWithinException(e.Message);
            }
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