using System.Collections.Generic;
using Worldpay.Innovation.WPWithin.Utils;

namespace Worldpay.Innovation.WPWithin
{
    public class Service
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Dictionary<int, Price> Prices { get; set; }

        public override int GetHashCode()
        {
            return new HashCodeBuilder<Service>(this).With(m => m.Id)
                .With(m => m.Name)
                .With(m => m.Description)
                .HashCode;
        }

        public override bool Equals(object that)
        {
            return new EqualsBuilder<Service>(this, that)
                .With(m => m.Id)
                .With(m => m.Name)
                .With(m => m.Description)
                .Equals();
        }

        public override string ToString()
        {
            return new ToStringBuilder<Service>(this)
                .Append(m => m.Id)
                .Append(m => m.Name)
                .Append(m => m.Description)
                .ToString();
        }
    }
}