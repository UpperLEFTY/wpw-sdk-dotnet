using System;
using System.Threading.Tasks;
using Common.Logging;
using Thrift.Server;
using Thrift.Transport;
using Worldpay.Innovation.WPWithin.AgentManager;
using Worldpay.Innovation.WPWithin.Rpc;
using Worldpay.Innovation.WPWithin.ThriftAdapters;

namespace Worldpay.Innovation.WPWithin.EventListener
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

        public void beginServiceDelivery(int serviceId, Rpc.Types.ServiceDeliveryToken serviceDeliveryToken,
            int unitsToSupply)
        {
            Log.DebugFormat(
                "BeginServiceDelivery invoked (serviceId={0}, serviceDeliveryToken={1}, unitsToSupply={2})", serviceId,
                serviceDeliveryToken, unitsToSupply);
            BeginServiceDelivery?.Invoke(serviceId, ServiceDeliveryTokenAdapter.Create(serviceDeliveryToken),
                unitsToSupply);
        }

        public void endServiceDelivery(int serviceId, Rpc.Types.ServiceDeliveryToken serviceDeliveryToken,
            int unitsReceived)
        {
            Log.DebugFormat("EndServiceDelivery invoked (serviceId={0}, serviceDeliveryToken={1}, unitsToSupply={2})",
                serviceId, serviceDeliveryToken, unitsReceived);
            EndServiceDelivery?.Invoke(serviceId, ServiceDeliveryTokenAdapter.Create(serviceDeliveryToken),
                unitsReceived);
        }

        public event WPWithinService.EndServiceDeliveryHandler EndServiceDelivery;

        public event WPWithinService.BeginServiceDeliveryHandler BeginServiceDelivery;

        public void Start()
        {
            if (_server != null)
            {
                throw new InvalidOperationException("Cannot start server that has already been started");
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
            if (_server == null) throw new InvalidOperationException("Cannot stop server when it has not been started");
            _server.Stop();
            Log.Info("Asked Thrift server to stop, now waiting for task to finish");
            _serverTask.Wait();
        }
    }
}