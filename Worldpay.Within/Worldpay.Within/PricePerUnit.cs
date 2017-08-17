using System;
using Worldpay.Innovation.WPWithin.Utils;

namespace Worldpay.Innovation.WPWithin
{
    /// <summary>
    ///     The price of a single unit of the service being described by a <see cref="Price" />.  Immutable.
    /// </summary>
    public struct PricePerUnit
    {
        /// <summary>
        ///     Creates a new immutable instance.
        /// </summary>
        /// <param name="amount">The numeric amount, must be >=0.</param>
        /// <param name="currencyCode">
        ///     The ISO4217 currency code.  Must not be null or zero length.  No validity checking is
        ///     performed.
        /// </param>
        public PricePerUnit(int amount, string currencyCode)
        {
            if (amount < 0) throw new ArgumentException("must be >=0", nameof(amount));
            if (string.IsNullOrEmpty(currencyCode))
                throw new ArgumentException("must be a string with length > 0", nameof(currencyCode));
            Amount = amount;
            CurrencyCode = currencyCode;
        }

        /// <summary>
        ///     The amount of currency that will be charged.
        /// </summary>
        public int Amount { get; }

        /// <summary>
        ///     The ISO4217 currency code for the price per unit.
        /// </summary>
        /// <example>GBP</example>
        public string CurrencyCode { get; }

        /// <summary>
        ///     Equality check based on <see cref="Amount" /> and <see cref="CurrencyCode" />.
        /// </summary>
        /// <param name="that"></param>
        /// <returns>True if this and <paramref name="that" /> have the same <see cref="Amount" /> and <see cref="CurrencyCode" />.</returns>
        public override bool Equals(object that)
        {
            return new EqualsBuilder<PricePerUnit>(this, that)
                .With(m => m.Amount)
                .With(m => m.CurrencyCode).Equals();
        }

        /// <summary>
        ///     Hash code generator override.
        /// </summary>
        /// <returns>Integer based on <see cref="Amount" /> and <see cref="CurrencyCode" />.</returns>
        public override int GetHashCode()
        {
            return new HashCodeBuilder<PricePerUnit>(this)
                .With(m => m.Amount)
                .With(m => m.CurrencyCode).HashCode;
        }

        public override string ToString()
        {
            return new ToStringBuilder<PricePerUnit>(this)
                .Append(m => m.Amount)
                .Append(m => m.CurrencyCode)
                .ToString();
        }
    }
}