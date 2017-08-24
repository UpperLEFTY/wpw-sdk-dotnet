using System;
using System.Configuration;
using System.IO;
using Common.Logging;
using Thrift.Protocol;
using Thrift.Transport;
using Worldpay.Innovation.WPWithin.Utils;

namespace Worldpay.Innovation.WPWithin.AgentManager
{
    /// <summary>
    ///     Manages the configuration of an Thrift RPC Agent (passed to <code>rpc-client.exe</code>) or the
    ///     <see cref="WPWithinService" /> instance that will connect to it.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The Thrift RPC Agent manages the communication between producers and consumers.  If the WPWithin SDK is used to
    ///         create separate producers and consumers
    ///         then the communication path is: <code>Consumer -> RPC Agent -> network -> RPC Agent -> Producer</code>.
    ///     </para>
    ///     <para>
    ///         Parameters have hard-coded defaults in this class and these defaults can be overridden in application settings
    ///         (typically in app.config or web.config files), or
    ///         set directly on an instance of this class before passing to
    ///         <see cref="RpcAgentManager.StartThriftRpcAgentProcess()" />.
    ///     </para>
    /// </remarks>
    /// <seealso cref="RpcAgentManager" />
    public class RpcAgentConfiguration
    {
        private static readonly ILog Log = LogManager.GetLogger<RpcAgentConfiguration>();


        /// <summary>
        ///     The name of the environment variable that contains the home directory for the Worldpay Within SDK.
        ///     The RPC Agent will be looked for under the "bin" subdirectory.
        /// </summary>
        public static readonly string WpwHomeEnvVarName = "WPW_HOME";

        /// <summary>
        ///     The application config property name for the full file path to the Thrift RPC Agent.
        /// </summary>
        public static readonly string PathPropertyName = "ThriftRpcAgent.Path";

        /// <summary>
        ///     The application config property name for the host name to bind the Thrift RPC Agent to.
        /// </summary>
        public static readonly string HostProperty = "ThriftRpcAgent.Host";

        /// <summary>
        ///     The default value for the full file path to the Thrift RPC Agent.
        /// </summary>
        public static readonly string HostPropertyDefault = "127.0.0.1";

        /// <summary>
        ///     The application config property name for the port to launch the Thrift RPC Agent on.
        /// </summary>
        public static readonly string PortProperty = "ThriftRpcAgent.Port";

        /// <summary>
        ///     The default value for the port to launch the Thrift RPC Agent on.
        /// </summary>
        public static readonly int PortPropertyDefault = 9091;

        /// <summary>
        ///     The application config property name for the Thrift protocol to use to connect to the Thrift RPC Agent.
        /// </summary>
        public static readonly string ProtocolProperty = "ThriftRpcAgent.Protocol";

        /// <summary>
        ///     The application config property name for specifying the port to listen to callback on (if not set or 0 then
        ///     callbacks are disabled).
        /// </summary>
        public static readonly string CallbackPortProperty = "ThriftRpcAgent.CallbackPort";

        /// <summary>
        ///     The default value for the protocol to use to connect to the Thrift RPC Agent.
        /// </summary>
        public static readonly string ProtocolPropertyDefault = "binary";

        /// <summary>
        ///     The default value for the callback port to use by the Thrift RPC Agent.  Default value of 0 indicates that
        ///     callbacks are disabled.
        /// </summary>
        public static readonly int CallbackPortPropertyDefault = 0;

        private int? _callbackPort;

        private string _rpcAgentPath;

        /// <summary>
        ///     Stores override for service host.
        /// </summary>
        private string _serviceHost;

        private int? _servicePort;

        /// <summary>
        ///     Retrieves the RPC Agent host property from application config or provides default value.
        /// </summary>
        public string ServiceHost
        {
            get { return _serviceHost ?? ConfigurationManager.AppSettings[HostProperty] ?? HostPropertyDefault; }
            set { _serviceHost = value; }
        }

        /// <summary>
        ///     Retrieves the RPC Agent protocol property from application config or provides default value.
        /// </summary>
        public string Protocol
            => ConfigurationManager.AppSettings[ProtocolProperty] ?? ProtocolPropertyDefault;

        /// <summary>
        ///     Retrieves the RPC Agent port property from application config or provides default value.
        /// </summary>
        public int ServicePort
        {
            get
            {
                if (_servicePort.HasValue)
                {
                    return _servicePort.Value;
                }
                string portString = ConfigurationManager.AppSettings[PortProperty];
                int port;
                if (portString == null || !int.TryParse(portString, out port))
                {
                    port = PortPropertyDefault;
                }
                return port;
            }
            set { _servicePort = value; }
        }

        /// <summary>
        ///     Specifying the port to listen to callback on (if null/not set then callbacks are disabled).  0 indicates no
        ///     callbacks required.
        /// </summary>
        public int CallbackPort
        {
            get
            {
                if (_callbackPort.HasValue)
                {
                    return _callbackPort.Value;
                }
                string portString = ConfigurationManager.AppSettings[CallbackPortProperty];
                int port;
                if (portString == null || !int.TryParse(portString, out port))
                {
                    return CallbackPortPropertyDefault;
                }
                return port;
            }
            set { _callbackPort = value; }
        }

        /// <summary>
        ///     <para>The full path to the RPC Agent executable.  If left null the following logic will be used to work it out:</para>
        ///     <list type="number">
        ///         <item>
        ///             <description>Look in the current directory (have explicitly overridden with a local copy).</description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Search up the path from the current directory for <code>worldpay-within-sdk</code>,
        ///                 then look for applications/rpc-agent/ (i.e. you're a developer and have just built it).
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Checks environment variable WPW_HOME (<see cref="WpwHomeEnvVarName" />) then looks for a
        ///                 <code>bin</code> subdirectory.
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Retrieves the property RPC Agent Path (<see cref="PathPropertyName" />) from the application configuration,
        ///     or attempts to work it out by searching up from the current directory, looking for
        ///     <code>applications/rpc-agent/rpc-agent.exe</code>.
        /// </summary>
        public string Path
        {
            get
            {
                if (_rpcAgentPath != null)
                {
                    return _rpcAgentPath;
                }

                string agentFilename = RpcAgentFilenameGenerator.GetForCurrent();
                Log.Info("Searching for " + agentFilename);

                _rpcAgentPath = LookForRpcAgentIn("wpw-bin") ??
                                LookForRpcAgentIn(GetPathFromApplicationConfig()) ??
                                LookForRpcAgentIn(GetPathFromEnvironment());

                if (_rpcAgentPath == null)
                {
                    // Ok, it's not anywhere we can find, so throw an exception because nothing's going to work.
                    throw new RpcAgentException(
                        $"Could not find a RPC Agent executable named {agentFilename} anywhere");
                }
                return _rpcAgentPath;
            }

            set { _rpcAgentPath = value; }
        }


        /// <summary>
        ///     If true, a secure transport will be used by the RPC Agent.
        /// </summary>
        public bool Secure { get; set; }

        /// <summary>
        ///     The full path to the log file that the RPC Agent will write to.  If null, no log file will be written.
        /// </summary>
        public FileInfo LogFile { get; set; }

        /// <summary>
        ///     The logging level that the launched RPC Agent will output.  Valid values are shown in the list below.  Use commas
        ///     to separate if you want multiple levels.
        ///     <list type="bullet">
        ///         <item>
        ///             <term>
        ///                 <code>panic</code>
        ///             </term>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <code>fatal</code>
        ///             </term>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <code>error</code>
        ///             </term>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <code>warn</code>
        ///             </term>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <code>info</code>
        ///             </term>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <code>debug</code>
        ///             </term>
        ///         </item>
        ///     </list>
        /// </summary>
        public string LogLevel { get; set; }

        /// <summary>
        ///     Whether transmission is framed or not.
        /// </summary>
        /// <remarks>
        ///     See <a href="http://thrift-tutorial.readthedocs.io/en/latest/thrift-stack.html">the Thrift documentation</a>
        ///     for more information.
        /// </remarks>
        public bool Framed { get; set; }

        /// <summary>
        ///     Configuration file that the Thrift RPC Agent should load its configuration from.
        /// </summary>
        public FileInfo ConfigFile { get; set; }

        /// <summary>
        ///     Whether tranmission is buffered or not.
        /// </summary>
        /// <remarks>See <a href="https://thrift.apache.org/docs/concepts">the Thrift documentation</a> for more information.</remarks>
        public bool Buffered { get; set; }

        /// <summary>
        ///     Buffer size for tranmission.
        /// </summary>
        /// <remarks>Null indicates no value (no default, will be decided by the agent itself).</remarks>
        public int BufferSize { get; set; }

        /// <summary>
        ///     The name of the named pipe that will be used, if <see cref="Transport" /> is set to <code>namedpipe</code>.
        /// </summary>
        public string NamedPipeName { get; set; } = "thrift-agent";

        /// <summary>
        ///     The type of transport that will be used to communicate with the RPC Agent.  Default is <code>socket</code>.  Set to
        ///     <code>nameddpipe</code> to use named pipes.
        /// </summary>
        public string Transport { get; set; } = "socket";

        /// <summary>
        /// Handy constant, for use with <see cref="LogLevel"/>, to tell the RPC Agent to log all levels of messages.
        /// </summary>
        public static string LogLevelAll => "verbose,error,fatal,panic,warn,debug";

        /// <summary>
        ///     Looks for a file with name matching the RPC Agent for this architecture in the diredctory passed.
        /// </summary>
        /// <param name="dirToLookIn">The directory to look in, if null this returns null.</param>
        /// <returns>The full path to the RPC Agent if found, null otherwise.</returns>
        private string LookForRpcAgentIn(string dirToLookIn)
        {
            if (dirToLookIn == null)
            {
                return null;
            }
            FileInfo fi =
                new FileInfo(string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), dirToLookIn,
                    RpcAgentFilenameGenerator.GetForCurrent()));
            return DoesFileExist(fi) ? fi.FullName : null;
        }


        private string GetPathFromApplicationConfig()
        {
            string agentPath = ConfigurationManager.AppSettings[PathPropertyName];
            return agentPath;
        }

        private bool DoesFileExist(FileInfo file)
        {
            string filename = file.FullName;
            Log.Debug($"Does {filename} exist?");
            bool result = file.Exists;
            Log.Info($"File {filename} does {(result ? "" : "not ")}exist");
            return result;
        }

        private string GetPathFromEnvironment()
        {
            string wpwHome = Environment.GetEnvironmentVariable(WpwHomeEnvVarName);
            if (string.IsNullOrEmpty(wpwHome))
            {
                Log.Debug(
                    $"{WpwHomeEnvVarName} not set, so cannot use it to search for RPC Agent");
                return null;
            }
            DirectoryInfo wpwHomeDir;
            try
            {
                wpwHomeDir = new DirectoryInfo(wpwHome);
            }
            catch (ArgumentException)
            {
                Log.Warn(
                    $"{WpwHomeEnvVarName} set, but {wpwHome} is not a valid directory name.  Ignoring it.");
                return null;
            }
            if (!wpwHomeDir.Exists)
            {
                Log.Warn(
                    $"{WpwHomeEnvVarName} set, but directory {wpwHome} does not exist.  Ignoring it.");
                return null;
            }
            DirectoryInfo binDir = new DirectoryInfo(System.IO.Path.Combine(wpwHomeDir.FullName, "bin"));
            if (!binDir.Exists)
            {
                Log.Warn(
                    $"{WpwHomeEnvVarName} directory {wpwHomeDir} exists, but has no bin subdirectory.  Ignoring it.");
                return null;
            }
            return binDir.FullName;
        }

        /// <summary>
        ///     Inclues all properties.
        /// </summary>
        public override string ToString()
        {
            return new ToStringBuilder<RpcAgentConfiguration>(this)
                .Append(m => m.BufferSize)
                .Append(m => m.Buffered)
                .Append(m => m.CallbackPort)
                .Append(m => m.ConfigFile)
                .Append(m => m.Framed)
                .Append(m => m.LogFile)
                .Append(m => m.LogLevel)
                .Append(m => m.NamedPipeName)
                .Append(m => m.Path)
                .Append(m => m.Protocol)
                .Append(m => m.Secure)
                .Append(m => m.ServiceHost)
                .Append(m => m.ServicePort)
                .Append(m => m.Transport)
                .ToString();
        }

        internal string ToCommandLineArguments()
        {
            return string.Join(" ", // TODO Tidy this up so non-specified arguments don't leave an extra space
                FormatArgument(ArgNameBuffer, BufferSize),
                FormatArgument(ArgNameBuffered, Buffered),
                FormatArgument(ArgNameCallbackPort, CallbackPort),
                FormatArgument(ArgNameConfigFile, ConfigFile),
                FormatArgument(ArgNameFramed, Framed),
                FormatArgument(ArgNameLogLevel, LogLevel),
                FormatArgument(ArgNameLogFile, LogFile),
                FormatArgument(ArgNameSecure, Secure),
                FormatArgument(ArgNameHost, ServiceHost),
                FormatArgument(ArgNamePort, ServicePort),
                FormatArgument(ArgNameProtocol, Protocol)
            );
        }

        private string FormatArgument(string argumentName, object argumentValue)
        {
            if (
                (argumentValue == null)
                || (argumentValue is bool && !(bool) argumentValue)
                || (argumentValue is int && ((int) argumentValue == 0))
            )
            {
                return null;
            }

            string argVal = argumentValue.ToString();
            if (argVal.Contains(" "))
            {
                argVal = $@"""{argVal}""";
            }
            return $"-{argumentName} {argVal}";
        }


        /// <summary>
        ///     Creates a Thrift protocol object, using the class as described in <see cref="Protocol" />.
        /// </summary>
        /// <param name="transport">The transport that the protocol will be wrapped around.  Must not be null.</param>
        /// <returns>
        ///     A TProtocol object.  If the value for <see cref="Protocol" /> is invalid then <see cref="TBinaryProtocol" />
        ///     is used.
        /// </returns>
        public TProtocol GetThriftProtcol(TTransport transport)
        {
            TProtocol protocol;
            switch (Protocol)
            {
                case "compact":
                    protocol = new TCompactProtocol(transport);
                    break;
                case "json":
                    protocol = new TJSONProtocol(transport);
                    break;
                //                case "binary":
                default:
                    protocol = new TBinaryProtocol(transport);
                    break;
            }

            return protocol;
        }

        /// <summary>
        ///     Creates an instance of a <see cref="TServerTransport" /> based on the <see cref="Transport" /> property of this
        ///     instance.
        /// </summary>
        /// <remarks>
        ///     Currently only supports <code>namedpipe</code> and <code>socket</code>.  Any other value will be interpreted
        ///     as <code>socket</code>
        /// </remarks>
        /// <returns>Never returns null.</returns>
        public TServerTransport GetThriftServerTransport()
        {
            TServerTransport transport;
            switch (Transport)
            {
                case "namedpipe":
                    transport = new TNamedPipeServerTransport(NamedPipeName);
                    break;
                default:
                    transport = new TServerSocket(CallbackPort);
                    break;
            }
            return transport;
        }

        /// <summary>
        ///     Retrives the correct Thrift transport type based on the value in <see cref="Transport" />.
        /// </summary>
        /// <returns>
        ///     If <see cref="Transport" /> is <code>namedpipe</code> then a <see cref="TNamedPipeClientTransport" /> is used.  For
        ///     all other values
        ///     a <see cref="TSocket" /> is returned.  Depending on the value of <see cref="Framed" /> and <see cref="Buffered" />,
        ///     appropriate wrappers will be placed
        ///     around the transport.
        /// </returns>
        public TTransport GetThriftTransport()
        {
            TTransport transport;
            switch (Transport)
            {
                case "namedpipe":
                    transport = new TNamedPipeClientTransport(ServiceHost, NamedPipeName);
                    break;
                //                case "socket":
                default:
                    transport = new TSocket(ServiceHost, ServicePort);
                    break;
            }

            if (Framed)
            {
                transport = new TFramedTransport(transport);
            }

            if (Buffered)
            {
                transport = new TBufferedTransport((TStreamTransport) transport, BufferSize);
            }

            return transport;
        }

        #region RPC Agent command line arguments, taken from the Go source for RPC Agent

        private const string ArgNameConfigFile = "configfile";
        private const string ArgNameLogLevel = "loglevel";
        private const string ArgNameLogFile = "logfile";
        private const string ArgNameProtocol = "protocol";
        private const string ArgNameFramed = "framed";
        private const string ArgNameBuffered = "buffered";
        private const string ArgNameHost = "host";
        private const string ArgNamePort = "port";
        private const string ArgNameSecure = "secure";
        private const string ArgNameBuffer = "buffer";
        private const string ArgNameCallbackPort = "callbackport";

        #endregion
    }
}