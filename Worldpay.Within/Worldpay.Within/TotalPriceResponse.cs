using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldpay.Innovation.WPWithin.Utils;

namespace Worldpay.Innovation.WPWithin
{
    public class TotalPriceResponse
    {
        public string ServerId { get; set; }

        public string ClientId { get; set; }

        public int? PriceId { get; set; }

        public int? UnitsToSupply { get; set; }

        public int? TotalPrice { get; set; }

        public string PaymentReferenceId { get; set; }

        public string MerchantClientKey { get; set; }

        public override bool Equals(object obj)
        {
            return new EqualsBuilder<TotalPriceResponse>(this, obj)
                .With(m => m.ServerId)
                .With(m => m.ClientId)
                .With(m => m.PriceId)
                .With(m => m.UnitsToSupply)
                .With(m => m.TotalPrice)
                .With(m => m.PaymentReferenceId)
                .With(m => m.MerchantClientKey)
                .Equals();
        }

        public override int GetHashCode()
        {
            return new HashCodeBuilder<TotalPriceResponse>(this)
                .With(m => m.ServerId)
                .With(m => m.ClientId)
                .With(m => m.PriceId)
                .With(m => m.UnitsToSupply)
                .With(m => m.TotalPrice)
                .With(m => m.PaymentReferenceId)
                .With(m => m.MerchantClientKey)
                .HashCode;
        }

        public override string ToString()
        {
            return new ToStringBuilder<TotalPriceResponse>(this)
                .Append(m => m.ServerId)
                .Append(m => m.ClientId)
                .Append(m => m.PriceId)
                .Append(m => m.UnitsToSupply)
                .Append(m => m.TotalPrice)
                .Append(m => m.PaymentReferenceId)
                .Append(m => m.MerchantClientKey)
                .ToString();

        }

    }
}
