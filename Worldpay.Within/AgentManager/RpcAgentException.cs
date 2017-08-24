using System;
using System.Runtime.Serialization;

namespace Worldpay.Innovation.WPWithin.AgentManager
{
    public class RpcAgentException : Exception
    {
        public RpcAgentException() : base()
        {
        }

        public RpcAgentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RpcAgentException(string message) : base(message)
        {
        }

        public RpcAgentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RpcAgentException(string fmt, params object[] parameters) : base(string.Format(fmt, parameters))
        {
        }
    }
}