using System.Collections.Generic;
using System.Threading;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Worldpay.Innovation.WPWithin;
using Worldpay.Innovation.WPWithin.AgentManager;
using Worldpay.Innovation.WPWithin.ThriftAdapters;

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
            WPWithinService thriftClient = new WPWithinService(new RpcAgentConfiguration
            {
                ServicePort = 9091,
                LogLevel = "verbose,error,fatal,warn,debug"
            });
            thriftClient.SetupDevice("DotNet RPC client", "This is coming from C# via Thrift RPC.");
            Log.Info("Initialising Producer");
            thriftClient.InitProducer(new PspConfig
            {
                MerchantClientKey = "cl_key",
                MerchantServiceKey = "srv_key",
                HtePublicKey = "cl_key",
                HtePrivateKey = "srv_key"
            });
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


        [TestMethod]
        public void TestStopServiceBroadcast()
        {
            WPWithinService wpWithin = new WPWithinService(new RpcAgentConfiguration
            {
                LogLevel = "verbose,error,fatal,warn,debug"
            });
            wpWithin.SetupDevice("DotNet RPC client", "This is coming from C# via Thrift RPC.");
            Log.Info("Initialising Producer");
            wpWithin.InitProducer(new PspConfig
            {
                MerchantClientKey = "cl_key",
                MerchantServiceKey = "srv_key",
                HtePublicKey = "cl_key",
                HtePrivateKey = "srv_key"
            });
            wpWithin.StartServiceBroadcast(10000);
            Thread.Sleep(2000);
            wpWithin.StopServiceBroadcast();
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

