using Worldpay.Innovation.WPWithin.Utils;

namespace Worldpay.Innovation.WPWithin
{
    /// <summary>
    ///     Describes a purchasable service, offered by a producer.
    /// </summary>
    public class Price
    {
        /// <summary>
        ///     Initialises a new instance with a fixed <paramref name="id" /> (must be unique within an owning service).
        /// </summary>
        /// <param name="id"></param>
        public Price(int id)
        {
            Id = id;
        }

        /// <summary>
        ///     The unique identity of the price.
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///     A description of the price, in whatever language the producer chooses.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The price for a single deliverable unit of the service (for example, 1 kWh of electricity, 1 litre of water).
        /// </summary>
        public PricePerUnit PricePerUnit { get; set; }

        /// <summary>
        ///     An identity for then unit.
        /// </summary>
        public int? UnitId { get; set; }

        /// <summary>
        ///     A description of the unit, in whatever language the producer chooses.
        /// </summary>
        public string UnitDescription { get; set; }

        /// <summary>
        ///     Equality test that uses all all attributes of this instance.
        /// </summary>
        /// <param name="that">Object to test this instance against.</param>
        /// <returns></returns>
        public override bool Equals(object that)
        {
            return new EqualsBuilder<Price>(this, that)
                .With(m => m.Id)
                .With(m => m.Description)
                .With(m => m.PricePerUnit)
                .With(m => m.UnitId)
                .With(m => m.UnitDescription)
                .Equals();
        }

        /// <summary>
        ///     Creates a hash code based on immutable attributes of this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return new HashCodeBuilder<Price>(this)
                .With(m => m.Id)
                .HashCode;
        }

        /// <summary>
        ///     Provides a human-readable string that includes all attributes of this instance.
        /// </summary>
        /// <returns>A string length >0, never null.</returns>
        public override string ToString()
        {
            return new ToStringBuilder<Price>(this)
                .Append(m => m.Id)
                .Append(m => m.Description)
                .Append(m => m.PricePerUnit)
                .Append(m => m.UnitId)
                .Append(m => m.UnitDescription)
                .ToString();
        }
    }
}