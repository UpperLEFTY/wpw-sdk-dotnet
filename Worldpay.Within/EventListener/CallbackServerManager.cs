﻿using System;
using System.Threading.Tasks;
using Common.Logging;
using Thrift.Server;
using Thrift.Transport;
using Worldpay.Within.AgentManager;
using Worldpay.Within.Rpc;
using Worldpay.Within.Rpc.Types;
using Worldpay.Within.ThriftAdapters;

namespace Worldpay.Within.EventListener
{
    /// <summary>
    ///     This is the callback server manager that manages a Thrift server to receive callbacks from the WPWithin SDK.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The server manager is not exposed to SDK consumers, as far as they are concerned, they just register handlers
    ///         for the <see cref="WPWithinService.OnBeginServiceDelivery" />
    ///         and <see cref="WPWithinService.OnEndServiceDelivery" /> events (which actually delegate directory to
    ///         <see cref="BeginServiceDelivery" /> and <see cref="EndServiceDelivery" /> in this class.
    ///     </para>
    ///     <para>
    ///         This callback server is set up during the initialisation of <see cref="WPWithinService" /> (regardless of
    ///         whether callback handers are registered or not).
    ///     </para>
    ///     <para>Adaption between Thrift-generated types and SDK types is done here.</para>
    /// </remarks>
    internal class CallbackServerManager : WPWithinCallback.Iface
    {
        private readonly RpcAgentConfiguration _config;
        private static readonly ILog Log = LogManager.GetLogger<CallbackServerManager>();
        private TServer _server;
        private Task _serverTask;

        public CallbackServerManager(RpcAgentConfiguration config)
        {
            _config = config;
        }

        public void beginServiceDelivery(int serviceID, int servicePriceID, Rpc.Types.ServiceDeliveryToken serviceDeliveryToken, int unitsToSupply)
        {
            Log.DebugFormat("BeginServiceDelivery invoked (serviceId={0}, servicePriceID={1}, serviceDeliveryToken={2}, unitsToSupply={3})",
                                serviceID, servicePriceID, serviceDeliveryToken, unitsToSupply);
            BeginServiceDelivery?.Invoke(serviceID, servicePriceID, ServiceDeliveryTokenAdapter.Create(serviceDeliveryToken), unitsToSupply);
        }

        public void endServiceDelivery(int serviceId, Within.Rpc.Types.ServiceDeliveryToken serviceDeliveryToken,
            int unitsReceived)
        {
            Log.DebugFormat("EndServiceDelivery invoked (serviceId={0}, serviceDeliveryToken={1}, unitsToSupply={2})",
                serviceId, serviceDeliveryToken, unitsReceived);
            EndServiceDelivery?.Invoke(serviceId, ServiceDeliveryTokenAdapter.Create(serviceDeliveryToken),
                unitsReceived);
        }

        public void makePaymentEvent(int totalPrice, string orderCurrency, string clientToken, string orderDescription, string uuid)
        {
            Log.DebugFormat("MakePaymentEvent invoked (totalPrice={0}, orderCurrency={1}, clientToken={2}, orderDescription={3}, uuid={4})",
                totalPrice, orderCurrency, clientToken, orderDescription, uuid);
            MakePaymentEvent?.Invoke(totalPrice, orderCurrency, clientToken, orderDescription, uuid);
        }

        public void serviceDiscoveryEvent(string remoteAddr)
        {
            Log.DebugFormat("ServiceDiscoveryEvent invoked (remoteAddr={0})", remoteAddr);
            ServiceDiscoveryEvent?.Invoke(remoteAddr);
        }

        public void servicePricesEvent(string remoteAddr, int serviceId)
        {
            Log.DebugFormat("ServicePricesEvent invoked (remoteAddr={0}, serviceId{1})", remoteAddr, serviceId);
            ServicePricesEvent?.Invoke(remoteAddr, serviceId);
        }

        public void serviceTotalPriceEvent(string remoteAddr, int serviceID, Rpc.Types.TotalPriceResponse totalPriceResp)
        {
            Log.DebugFormat("ServiceTotalPriceEvent invoked (remoteAddr={0}, serviceID{1})", remoteAddr, serviceID);
            ServiceTotalPriceEvent?.Invoke(remoteAddr, serviceID, TotalPriceResponseAdapter.Create(totalPriceResp));
        }

        public void errorEvent(string msg)
        {
            Log.DebugFormat("ErrorEvent invoked (msg={0})", msg);
            ErrorEvent?.Invoke(msg);
        }

        public event WPWithinService.EndServiceDeliveryHandler EndServiceDelivery;

        public event WPWithinService.BeginServiceDeliveryHandler BeginServiceDelivery;

        public event WPWithinService.MakePaymentEventHandler MakePaymentEvent;

        public event WPWithinService.ServiceDiscoveryEventHandler ServiceDiscoveryEvent;

        public event WPWithinService.ServicePricesEventHandler ServicePricesEvent;

        public event WPWithinService.ServiceTotalPriceEventHandler ServiceTotalPriceEvent;

        public event WPWithinService.ErrorEventHandler ErrorEvent;

        public void Start()
        {
            if (_server != null)
            {
                throw new InvalidOperationException("Cannot start callback server that has already been started");
            }
            WPWithinCallback.Processor processor = new WPWithinCallback.Processor(this);
            TServerTransport serverTransport = _config.GetThriftServerTransport();
            TServer server = new TThreadPoolServer(processor, serverTransport);

            Log.InfoFormat("Starting callback server on {0}", _config.ServiceHost);
            _serverTask = Task.Run(() => server.Serve());
            _server = server;
        }

        public void Stop()
        {
            if (_server == null) throw new InvalidOperationException("Cannot stop callback server when it has not been started");
            _server.Stop();
            Log.Info("Asked Thrift callback server to stop, now waiting for task to finish");
            _serverTask.Wait();
        }
    }
}