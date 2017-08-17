using System.Collections.Generic;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Worldpay.Innovation.WPWithin;
using Worldpay.Innovation.WPWithin.AgentManager;

namespace Worldpay.Within.Tests
{
    [TestClass]
    public class ProducerTest
    {
        private static readonly ILog Log = LogManager.GetLogger<ProducerTest>();
        private static RpcAgentManager _mgr;


        [TestMethod]
        public void SendSimpleMessage()
        {
            WPWithinService thriftClient = new WPWithinService(new RpcAgentConfiguration());
            thriftClient.SetupDevice("DotNet RPC client", "This is coming from C# via Thrift RPC.");
            Log.Info("Initialising Producer");
            thriftClient.InitProducer("cl_key", "srv_key");
            thriftClient.StartServiceBroadcast(2000);
            IEnumerable<ServiceMessage> svcMsgs = thriftClient.DeviceDiscovery(2000);

            if (svcMsgs != null)
            {
                foreach (ServiceMessage svcMsg in svcMsgs)
                {
                    Log.InfoFormat("{0} - {1} - {2} - {3}", svcMsg.DeviceDescription, svcMsg.Hostname, svcMsg.PortNumber,
                        svcMsg.ServerId);
                }
            }
            else
            {
                Log.Info("Broadcast ok, but no services found");
            }
            Log.Info("All done, closing transport");
        }

        [TestInitialize]
        public void StartThriftRpcService()
        {
            _mgr = new RpcAgentManager(new RpcAgentConfiguration());
            _mgr.StartThriftRpcAgentProcess();
        }

        [TestCleanup]
        public void StopThriftRpcService()
        {
            _mgr.StopThriftRpcAgentProcess();
        }
    }
}

