using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Worldpay.Innovation.WPWithin.AgentManager;

namespace Worldpay.Within.Tests
{
    [TestClass]
    public class RpcAgentConfigurationTest
    {
        [TestMethod]
        public void TestPathOverride()
        {
            RpcAgentConfiguration cfg = new RpcAgentConfiguration();
            const string filename = "some random string";
            cfg.Path = filename;
            Assert.AreEqual(filename, cfg.Path);
        }
        
        [TestMethod]
        public void TestRpcAgentFilenameGenerator()
        {
            Assert.AreEqual("rpc-agent-windows-amd64.exe",
                RpcAgentFilenameGenerator.GetForEnvironment(ProcessorArchitecture.Amd64, PlatformID.Win32Windows, true));
            Assert.AreEqual("rpc-agent-windows-386.exe",
                RpcAgentFilenameGenerator.GetForEnvironment(ProcessorArchitecture.X86, PlatformID.Win32Windows, false));
            Assert.AreEqual("rpc-agent-linux-arm64",
                RpcAgentFilenameGenerator.GetForEnvironment(ProcessorArchitecture.Arm, PlatformID.Unix, true));
            Assert.AreEqual("rpc-agent-linux-arm32",
                RpcAgentFilenameGenerator.GetForEnvironment(ProcessorArchitecture.Arm, PlatformID.Unix, false));
            Assert.AreEqual("rpc-agent-linux-386",
                RpcAgentFilenameGenerator.GetForEnvironment(ProcessorArchitecture.X86, PlatformID.Unix, false));
            Assert.AreEqual("rpc-agent-linux-amd64",
                RpcAgentFilenameGenerator.GetForEnvironment(ProcessorArchitecture.Amd64, PlatformID.Unix, true));
            Assert.AreEqual("rpc-agent-darwin-386",
                RpcAgentFilenameGenerator.GetForEnvironment(ProcessorArchitecture.X86, PlatformID.MacOSX, false));
            Assert.AreEqual("rpc-agent-darwin-amd64",
                RpcAgentFilenameGenerator.GetForEnvironment(ProcessorArchitecture.Amd64, PlatformID.MacOSX, true));
        }
    }
}