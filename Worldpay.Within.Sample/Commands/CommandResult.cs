using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldpay.Innovation.WPWithin.Sample.Commands
{

    /// <summary>
    /// Used by <see cref="CommandMenu"/> commands to indicate how the method terminated (if an exception wasn't thrown).
    /// </summary>
    public enum CommandResult
    {
        /// <summary>
        /// No operation
        /// </summary>
        NoOp,
        /// <summary>
        /// Invalid command
        /// </summary>
        NoSuchCommand,
        /// <summary>
        /// Command executed successfully
        /// </summary>
        Success,
        /// <summary>
        /// An error occurred, which the program can continue (to try again).
        /// </summary>
        NonCriticalError,
        /// <summary>
        /// A critical error occurred and should cause the calling program to terminate.
        /// </summary>
        CriticalError,
        /// <summary>
        /// Calling program should terminate.
        /// </summary>
        Exit
    }

}
