using System;
using System.IO;
using System.Threading;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thrift;
using Worldpay.Innovation.WPWithin;
using Worldpay.Innovation.WPWithin.AgentManager;
using Worldpay.Innovation.WPWithin.ThriftAdapters;

namespace Worldpay.Within.Tests
{
    [TestClass]
    public class PspConfigTest
    {

        private static readonly ILog Log = LogManager.GetLogger<PspConfigTest>();

        private static RpcAgentManager _mgr;

        /// <summary>
        /// Should get an error when HTE Credentials are not passed to the PSPConfig
        /// 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(WPWithinException))]
        public void TestMissingHteCredentials()
        {
            WPWithinService thriftClient = new WPWithinService(new RpcAgentConfiguration
            {
                ServicePort = 9091,
                LogLevel = RpcAgentConfiguration.LogLevelAll,
                LogFile = new FileInfo("testmissinghtecredentials.log"),
            });
//            thriftClient.SetupDevice("DotNet RPC client", "This is coming from C# via Thrift RPC.");
            thriftClient.SetupDevice(null, null);
            Log.Info("Initialising Producer");
            thriftClient.InitProducer(new PspConfig
            {
                MerchantClientKey = "cl_key",
                MerchantServiceKey = "srv_key",
            });
            Log.Info("Producer initalised successfully");
            Thread.Sleep(2000);
//            Log.Info("Starting service broadcast");
//            thriftClient.StartServiceBroadcast(2000);
//            Log.Info("Done Service broadcast");

        }
        [TestInitialize]
        public void StartThriftRpcService()
        {
            _mgr = new RpcAgentManager(new RpcAgentConfiguration
            {
                LogLevel = "verbose,error,fatal,warn,debug",
                LogFile = new FileInfo("testmissinghtecredentials.log"),
            });
            _mgr.StartThriftRpcAgentProcess();
        }

        [TestCleanup]
        public void StopThriftRpcService()
        {
            _mgr.StopThriftRpcAgentProcess();
        }

        [TestMethod]
        [ExpectedException(typeof(WPWithinException))]
        public void TestException()
        {
            WPWithinService thriftClient = new WPWithinService(new RpcAgentConfiguration
            {
                ServicePort = 9091,
                LogLevel = "verbose,error,fatal,warn,debug",
                LogFile = new FileInfo("testmissinghtecredentials.log"),
            });
            thriftClient.SetupDevice(null, null);

            Log.Info("Initialising Producer");
            thriftClient.InitProducer(new PspConfig());
        }

    }
}
