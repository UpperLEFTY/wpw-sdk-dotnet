using System;
using System.Reflection;
using Common.Logging;

namespace Worldpay.Innovation.WPWithin.AgentManager
{
    /// <summary>
    /// Utility methods to work out the name of the RPC agent executable for a given platform.
    /// </summary>
    public class RpcAgentFilenameGenerator
    {
        private static readonly ILog Log = LogManager.GetLogger<RpcAgentManager>();

        /// <summary>
        /// Suppress instantiation.
        /// </summary>
        private RpcAgentFilenameGenerator()
        {
        }

        /// <summary>
        ///     Determines the correct RPC Agent filename, based on the characteristics of the environment in which this wrapper is
        ///     running.
        /// </summary>
        /// <remarks>
        ///     The existing available RPC Agents (from the 0.4 build) are:
        ///     <list type="bullet">
        ///         <item>
        ///             <term>rpc-agent-linux-amd64</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-linux-arm32</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-linux-arm64</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-darwin-386</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-darwin-amd64</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-linux-386</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-windows-386.exe</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-windows-amd64.exe</term>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <returns>A filename</returns>
        /// <exception cref="RpcAgentException">If a filename could not be found for this platform.</exception>
        public static string GetForCurrent()
        {
            ProcessorArchitecture architecture = typeof(RpcAgentFilenameGenerator).Assembly.GetName().ProcessorArchitecture;
            PlatformID platform = Environment.OSVersion.Platform;
            return GetForEnvironment(architecture, platform, Environment.Is64BitOperatingSystem);
        }


        /// <summary>
        ///     Determines the correct RPC Agent filename based on a platform, architecture and address size passed.
        /// </summary>
        /// <remarks>
        ///     The existing available RPC Agents (from the 0.4 build) are:
        ///     <list type="bullet">
        ///         <item>
        ///             <term>rpc-agent-linux-amd64</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-linux-arm32</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-linux-arm64</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-darwin-386</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-darwin-amd64</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-linux-386</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-windows-386.exe</term>
        ///         </item>
        ///         <item>
        ///             <term>rpc-agent-windows-amd64.exe</term>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="architecture">The architecture that the RPC Agent will be running on.</param>
        /// <param name="platformId">The operating system/platform that the RPC Agent will be running on.</param>
        /// <param name="is64Bit">True if the RPC Agent will be running on a 64-bit architecture, if alse 32-bit is assumed.</param>
        /// <returns>A filename</returns>
        /// <exception cref="RpcAgentException">If a filename could not be found for this platform.</exception>
        public static string GetForEnvironment(ProcessorArchitecture architecture,
            PlatformID platformId,
            bool is64Bit)
        {
            string platformSuffix;
            string executableSuffix;
            switch (platformId)
            {
                case PlatformID.MacOSX:
                    platformSuffix = "darwin";
                    executableSuffix = null;
                    break;
                case PlatformID.Unix:
                    platformSuffix = "linux";
                    executableSuffix = null;
                    break;
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    platformSuffix = "windows";
                    executableSuffix = ".exe";
                    break;
                default:
                    throw new RpcAgentException($"Platform {platformId} is not a support platform for the RPC Agent");
            }

            string architectureSuffix;
            switch (architecture)
            {
                case ProcessorArchitecture.Arm:
                    architectureSuffix = "arm" + (is64Bit ? "64" : "32");
                    break;
                default:
                    architectureSuffix = (is64Bit ? "amd64" : "386");
                    break;
            }

            string rpcAgentFilename = $"rpc-agent-{platformSuffix}-{architectureSuffix}{executableSuffix}";

            Log.InfoFormat("RPC Agent for this platform and architecture is {0}", rpcAgentFilename);

            return rpcAgentFilename;
        }
    }
}