using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Common.Logging;

namespace Worldpay.Innovation.WPWithin.AgentManager
{
    /// <summary>
    ///     Manages the lifecycle of a Thrift RPC Agent (see /applications/rpc-agent).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The Thift RPC agent is what this code talks to in order to communicate with the other participant in the
    ///         conversation.
    ///     </para>
    ///     <para>
    ///         For example, if we are writing a .NET producer, we commnunicate with the consumer by invoking the Thrift RPC
    ///         Agent (typically
    ///         located locally) which then talks to the consumer service (typically located remotely).
    ///     </para>
    ///     <para>
    ///         As the Thrift RPC Agent runs as separate process, we have to invoke it as if it were a separate tool, rather
    ///         than loading in to the same address space.
    ///     </para>
    /// </remarks>
    public class RpcAgentManager
    {
        /// <summary>
        ///     Delegate used for <see cref="RpcAgentManager.OnMessage" /> and <see cref="RpcAgentManager.OnError" />.
        /// </summary>
        /// <param name="process">The process that the RPC Agent process is running under.</param>
        /// <param name="message">The message received from the RPC Agent process.</param>
        public delegate void ThriftRpcAgentOutput(Process process, string message);

        private static readonly ILog Log = LogManager.GetLogger<RpcAgentManager>();
        private static readonly ILog ThriftRpcLog = LogManager.GetLogger("ThriftRpcAgent");

        /// <summary>
        ///     The name of the environment variable that notes the Worldpay Within home directory.  The RPC Agent binary
        ///     will be looked for in a bin subdirectory.
        /// </summary>
        public static readonly string RpcAgentEnvironmentVariableName = "WPW_HOME";

        private readonly RpcAgentConfiguration _config;

        private Process _thriftRpcProcess;

        /// <summary>
        ///     No-op except for storing the passed config.
        /// </summary>
        /// <param name="config"></param>
        public RpcAgentManager(RpcAgentConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        ///     Invoked whenever a message is sent to the RPC Agent process's standard output stream.
        /// </summary>
        public event ThriftRpcAgentOutput OnMessage;

        /// <summary>
        ///     Invoked whenever a message is sent ot the RPC Agent process's standard error stream.
        /// </summary>
        public event ThriftRpcAgentOutput OnError;

        /// <summary>
        ///     Invoked when the Thrift RPC Agent has successfully been started.
        /// </summary>
        public event EventHandler OnStarted;

        /// <summary>
        ///     Invoked when the Thrift RPC Agent has successfully been stopped, or has crashed out.
        /// </summary>
        public event EventHandler OnExited;

        /// <summary>
        ///     Starts the Thrift RPC Agent Process with the configuration passed in the constructor.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method launches a sub-process to start the Thrift RPC Agent.  Before calling this, you should add an event
        ///         handler for
        ///         the <see cref="OnStarted" /> event.  Until this event is invoked, it is not guaranteed that the
        ///         process has started.
        ///     </para>
        ///     <para>If the agent fails to start properly then the <see cref="OnExited" />.</para>
        /// </remarks>
        public void StartThriftRpcAgentProcess()
        {
            string arguments = _config.ToCommandLineArguments();
            Log.InfoFormat("Launching Thift RPC Agent with args: {0} {1}",
                _config.Path, arguments);

            Process thriftRpcProcess = new Process
            {
                StartInfo = new ProcessStartInfo(_config.Path, arguments)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            thriftRpcProcess.OutputDataReceived += ThriftRpcProcess_OutputDataReceived;
            thriftRpcProcess.ErrorDataReceived += ThriftRpcProcess_ErrorDataReceived;
            thriftRpcProcess.Exited += ThriftRpcProcess_Exited;
            thriftRpcProcess.Start();
            thriftRpcProcess.BeginOutputReadLine();
            thriftRpcProcess.BeginErrorReadLine();
            _thriftRpcProcess = thriftRpcProcess;
        }

        private void ThriftRpcProcess_Exited(object sender, EventArgs e)
        {
            Process proc = (Process) sender;
            if (proc.ExitCode == 0)
                ThriftRpcLog.Info("Thrift RPC Agent has terminated with exit code 0");
            else
                ThriftRpcLog.Fatal($"Thrift RPC Agent has exited abnormally with exit code {proc.ExitCode}");
            OnExited?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        ///     Terminates the Thrift RPC Agent.
        /// </summary>
        /// <remarks>
        ///     Attempts to send a Ctrl-C signal to the agent.  If this cannot be done, or fails, then the process is killed via
        ///     <see cref="Process.Kill" />.
        /// </remarks>
        public void StopThriftRpcAgentProcess()
        {
            if (_thriftRpcProcess == null)
                throw new InvalidOperationException("Cannot stop Thrift RPC Agent process unless it's started");
            Log.InfoFormat("Attempting to stop Thrift RPC Agent {0}", _thriftRpcProcess.Id);

            if (_thriftRpcProcess.HasExited)
            {
                Log.Warn("Ignoring call to stop Thrift RPC agent as the process has already exited");
            }
            else
            {
                // BUG Commented out Ctrl-C style kill code as it won't work if in a Console app, will re-enable once I've fixed it properly
                //                if (!SentCtrlCToProcess(_thriftRpcProcess))
                //                {
                Log.Info("Unable to gracefully stop Thift RPC Agent, issuing kill instead");
                _thriftRpcProcess.Kill();
                //                }
            }
        }


        private void ThriftRpcProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Process process = (Process) sender;
            ThriftRpcLog.Error($"RpcAgent({process.Id}): {e.Data}");
            OnError?.Invoke(process, e.Data);
        }


        private void ThriftRpcProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Process process = (Process) sender;
            ThriftRpcLog.Info($"RpcAgent({process.Id}): {e.Data}");
            OnMessage?.Invoke(process, e.Data);
            /* startString is a magic string that we look for in the RPC Agent startup that lets us know
             * that it's ready for calls.  This is the last message emitted on successful startup. */
            const string startString = "Begin log setup";
            if (e.Data != null && e.Data.Contains(startString))
            {
                Log.InfoFormat("Found magic string \"{0}\" indicating that RPC Agent {1} has started successfully",
                    startString, process.Id);
                OnStarted?.Invoke(this, EventArgs.Empty);
            }
        }

        #region Managing Process shutdown gracefully (Ctrl+C instead of kill)

        internal const int CtrlCEvent = 0;

        [DllImport("kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handlerRoutine, bool add);

        // Delegate type to be used as the Handler Routine for SCCH
        private delegate bool ConsoleCtrlDelegate(uint ctrlType);

        private bool SentCtrlCToProcess(Process process)
        {
            // BUG The problem is, if this is a command line app, then FreeConsole will disconnect our own console
            FreeConsole();

            if (AttachConsole((uint) process.Id))
            {
                // Stop the Ctrl-C from terminating this process
                SetConsoleCtrlHandler(null, true);
                try
                {
                    // Attempt to send a Ctrl-C to the thrift RPC agent process
                    if (!GenerateConsoleCtrlEvent(CtrlCEvent, 0))
                    {
                        Log.Warn("Unable to generate Ctrl-C event for process");
                        return false;
                    }
                    Log.Info("Sent Ctrl-C to Thrift RPC agent, now waiting for the process to terminate.");
                    process.WaitForExit();
                    Log.Info("Thrift RPC agent has exited cleanly.");
                }
                finally
                {
                    // Always restore the default Ctrl-C handler so that we honour a Ctrl-C from the user.
                    Log.Debug("Restoring Ctrl-C default handler for this process");
                    FreeConsole();
                    SetConsoleCtrlHandler(null, false);
                }
                return true;
            }
            else
            {
                Log.Warn($"Unable to attach to console on process {process.Id}");
                return false;
            }
        }

        #endregion
    }
}