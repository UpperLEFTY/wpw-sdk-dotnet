using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldpay.Innovation.WPWithin.Utils;

namespace Worldpay.Innovation.WPWithin
{
    public class ServiceDeliveryToken
    {
        public string Key { get; set; }

        public string Issued { get; set; }

        public string Expiry { get; set; }

        public bool? RefundOnExpiry { get; set; }

        public byte[] Signature { get; set; }

        public override bool Equals(object that)
        {
            return new EqualsBuilder<ServiceDeliveryToken>(this, that)
                .With(m => m.Key)
                .With(m => m.Issued)
                .With(m => m.Expiry)
                .With(m => m.RefundOnExpiry)
                .With(m => m.Signature).Equals();
        }

        public override int GetHashCode()
        {
            return new HashCodeBuilder<ServiceDeliveryToken>(this)
                .With(m => m.Key)
                .With(m => m.Issued)
                .With(m => m.Expiry)
                .With(m => m.RefundOnExpiry)
                .With(m => m.Signature)
                .HashCode;
        }

        public override string ToString()
        {
            return new ToStringBuilder<ServiceDeliveryToken>(this)
                .Append(m => m.Key)
                .Append(m => m.Issued)
                .Append(m => m.Expiry)
                .Append(m => m.RefundOnExpiry)
                .Append(m => m.Signature)
                .ToString();
        }
    }
}
