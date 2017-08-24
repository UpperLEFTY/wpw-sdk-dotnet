using Worldpay.Innovation.WPWithin.Utils;

namespace Worldpay.Innovation.WPWithin
{

    /// <summary>
    /// Describes a remote service/device that we can connect to in order to consumer a service they are producing.  
    /// </summary>
    /// <see cref="Device"/> 
    public class ServiceMessage
    {
        public ServiceMessage(string serverId, string urlPrefix, int? portNumber, string hostname, string deviceDescription)
        {
            ServerId = serverId;
            UrlPrefix = urlPrefix;
            PortNumber = portNumber;
            Hostname = hostname;
            DeviceDescription = deviceDescription;
        }

        public string DeviceDescription { get; }

        public string Hostname { get; }

        public int? PortNumber { get; }

        public string ServerId { get;}

        public string UrlPrefix { get; }

        public override bool Equals(object that)
        {
            return new EqualsBuilder<ServiceMessage>(this, that)
                .With(m => m.DeviceDescription)
                .With(m => m.Hostname)
                .With(m => m.PortNumber)
                .With(m => m.ServerId)
                .With(m => m.UrlPrefix)
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
