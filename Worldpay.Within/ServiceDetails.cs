using Worldpay.Innovation.WPWithin.Utils;

namespace Worldpay.Innovation.WPWithin
{
    /// <summary>
    /// Describes a service offered by a device.
    /// </summary>
    public class ServiceDetails
    {

        /// <summary>
        /// Constructs a new instance with the service Id specified.
        /// </summary>
        public ServiceDetails(int serviceId)
        {
            ServiceId = serviceId;
        }

        /// <summary>
        /// The unique (within the device) identity of the service offered by the device.
        /// </summary>
        public int ServiceId { get; }

        /// <summary>
        /// A human readable description of the service.
        /// </summary>
        public string ServiceDescription { get; set; }

        /// <summary>
        /// Equality check based on attributes of instance.
        /// </summary>
        public override bool Equals(object that)
        {
            return new EqualsBuilder<ServiceDetails>(this, that)
                .With(m => m.ServiceId)
                .With(m => m.ServiceDescription)
                .Equals();
        }

        /// <summary>
        /// Generates hash code based on immutable attributes of instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return new HashCodeBuilder<ServiceDetails>(this)
                .With(m => m.ServiceId)
                .HashCode;
        }

        /// <summary>
        /// Creates human-readable string the contains all attributes.
        /// </summary>
        public override string ToString()
        {
            return new ToStringBuilder<ServiceDetails>(this)
                .Append(m => m.ServiceId)
                .Append(m => m.ServiceDescription)
                .ToString();
        }
    }
}
