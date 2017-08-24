using System.Threading;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Worldpay.Innovation.WPWithin;
using Worldpay.Innovation.WPWithin.AgentManager;

namespace Worldpay.Within.Tests
{

    /// <summary>
    /// Tests that the RPC Agent Manager can be started and stopped by direct invocation or event.
    /// </summary>
    [TestClass]
    public class StartStopTest
    {
        private static readonly ILog Log = LogManager.GetLogger<StartStopTest>();

        [TestMethod]
        public void StartAndStop()
        {
            RpcAgentConfiguration cfg = new RpcAgentConfiguration
            {
                CallbackPort = 9092,
                ServicePort = 9091,
                ServiceHost = "localhost"
            };
            RpcAgentManager mgr = new RpcAgentManager(cfg);
            mgr.StartThriftRpcAgentProcess();
            try
            {
                using (WPWithinService service = new WPWithinService(cfg))
                {
                    Log.InfoFormat("Successfully connected {0} to Thrift RPC Agent on {1}", service, cfg);
                }
            }
            finally
            {
                mgr.StopThriftRpcAgentProcess();
            }
        }

        [TestMethod]
        public void StartAndStopViaEvent()
        {
            RpcAgentManager mgr = new RpcAgentManager(new RpcAgentConfiguration());

            bool started = false;
            mgr.OnStarted += (s, e) =>
            {
                started = true;
                Log.Info("Started RPC Agent manager ok, now stopping it");
                mgr.StopThriftRpcAgentProcess();
            };
            Log.Info("Starting RPC Agent manager with default configuration");
            mgr.StartThriftRpcAgentProcess();

            int retries = 0;
            while (!started && retries < 10)
            {
                Thread.Sleep(500);
                retries++;
            }
            if (!started) Assert.Fail("Thrift RPC Agent didn't start within 5000ms");
            Log.Info("RPC Agent manager confirmed terminated");
        }


    }
}
