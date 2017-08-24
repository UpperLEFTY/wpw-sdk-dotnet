using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Worldpay.Innovation.WPWithin;
using Worldpay.Innovation.WPWithin.AgentManager;
using Worldpay.Innovation.WPWithin.ThriftAdapters;

namespace Worldpay.Within.Tests
{
    [TestClass]
    public class PaymentTest
    {
        [TestMethod]
        public void TestSinglePayment()
        {
            PspConfig pspConfig = new PspConfig()
            {
                HtePublicKey = "T_C_03eaa1d3-4642-4079-b030-b543ee04b5af",
                HtePrivateKey = "T_S_f50ecb46-ca82-44a7-9c40-421818af5996",
                MerchantClientKey = "T_C_03eaa1d3-4642-4079-b030-b543ee04b5af",
                MerchantServiceKey = "T_S_f50ecb46-ca82-44a7-9c40-421818af5996"
            };

            RpcAgentConfiguration producerAgentConfig = new RpcAgentConfiguration
            {
                ServicePort = 9091,
                LogFile = new FileInfo("testSinglePayment_Producer.log"),
                LogLevel = "verbose,error,fatal,warn,debug"
            };
            RpcAgentManager producerAgent = new RpcAgentManager(producerAgentConfig);

            RpcAgentConfiguration consumerAgentConfig = new RpcAgentConfiguration
            {
                ServicePort = 9092,
                LogFile = new FileInfo("testSinglePayment_Consunmer.log"),
                LogLevel = "verbose,error,fatal,warn,debug"
            };
            RpcAgentManager consumerAgent = new RpcAgentManager(consumerAgentConfig);

            producerAgent.StartThriftRpcAgentProcess();
            try
            {
                consumerAgent.StartThriftRpcAgentProcess();
                try
                {
                    using (WPWithinService producer = new WPWithinService(producerAgentConfig))
                    {
                        using (WPWithinService consumer = new WPWithinService(consumerAgentConfig))
                        {
                            producer.SetupDevice("DotNetProducer", "TestSinglePayment Producer Unit Test");
                            consumer.SetupDevice("DotNetConsumer", "TestSinglePayment Consumer Unit Test");
                        }
                    }
                }
                finally
                {
                    consumerAgent.StopThriftRpcAgentProcess();
                }
            }
            finally
            {
                producerAgent.StopThriftRpcAgentProcess();
            }

        }
    }
}
