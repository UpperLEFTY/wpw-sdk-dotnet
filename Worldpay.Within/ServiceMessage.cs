using System.Collections.Generic;
using Worldpay.Within.Utils;

namespace Worldpay.Within
{

    /// <summary>
    /// Describes a remote service/device that we can connect to in order to consumer a service they are producing.  
    /// </summary>
    /// <see cref="Device"/> 
    public class ServiceMessage
    {
        public ServiceMessage(string serverId, string urlPrefix, int? portNumber, string hostname, string deviceDescription,
            string scheme, string deviceName, HashSet<string> serviceTypes)
        {
            ServerId = serverId;
            UrlPrefix = urlPrefix;
            PortNumber = portNumber;
            Hostname = hostname;
            DeviceDescription = deviceDescription;
            Scheme = scheme;
            DeviceName = deviceName;
            ServiceTypes = serviceTypes;
        }

        public string DeviceDescription { get; }

        public string Hostname { get; }

        public int? PortNumber { get; }

        public string ServerId { get; }

        public string UrlPrefix { get; }

        public string Scheme { get; }

        public string DeviceName { get; }

        public HashSet<string> ServiceTypes { get; }

        public override bool Equals(object that)
        {
            return new EqualsBuilder<ServiceMessage>(this, that)
                .With(m => m.DeviceDescription)
                .With(m => m.Hostname)
                .With(m => m.PortNumber)
                .With(m => m.ServerId)
                .With(m => m.UrlPrefix)
                .With(m => m.Scheme)
                .With(m => m.DeviceName)
                .With(m => m.ServiceTypes)
                .Equals();
        }

        public override int GetHashCode()
        {
            return new HashCodeBuilder<ServiceMessage>(this)
                .With(m => m.DeviceDescription)
                .With(m => m.Hostname)
                .With(m => m.PortNumber)
                .With(m => m.ServerId)
                .With(m => m.UrlPrefix)
                .HashCode;
        }

        public override string ToString()
        {
            return new ToStringBuilder<ServiceMessage>(this)
                .Append(m => m.DeviceDescription)
                .Append(m => m.Hostname)
                .Append(m => m.PortNumber)
                .Append(m => m.ServerId)
                .Append(m => m.UrlPrefix)
                .ToString();
        }
    }
}
