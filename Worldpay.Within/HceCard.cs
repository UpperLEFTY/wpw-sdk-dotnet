using Worldpay.Innovation.WPWithin.Utils;

namespace Worldpay.Innovation.WPWithin
{
    /// <summary>
    /// Represents a Hosted Card Emulator, a payment instrument, such as a credit card, than can be used to purchase services from a producer.
    /// </summary>
    public class HceCard
    {

        /// <summary>
        /// Initialises a new immutable instance of a card.
        /// </summary>
        /// <param name="firstName">The first name that appears on the card.</param>
        /// <param name="lastName">The last name that appears on the card.</param>
        /// <param name="expMonth">The expiry month on the card.  <code>1</code> = January, <code>12</code> = December.</param>
        /// <param name="expYear">The expiry year on the card, for example <code>2018</code>.</param>
        /// <param name="cardNumber">The long card number, may include spaces.  E.g. <code>4444 3333 2222 1111</code>.</param>
        /// <param name="type">
        /// The type of payment instrument, this should always be <code>Card</code>, responses from the producer would t ypically be <code>ObfuscatedCard</code> (where
        /// all but the last 4 digits are represented by asterisks).
        /// </param>
        /// <param name="cvc">Card Verification Code, required for card not present transactions.</param>
        public HceCard(string firstName, string lastName, string type, string cardNumber, int? expMonth, int? expYear, string cvc)
        {
            FirstName = firstName;
            LastName = lastName;
            ExpMonth = expMonth;
            ExpYear = expYear;
            CardNumber = cardNumber;
            Type = type;
            Cvc = cvc;
        }

        /// <summary>
        /// The first name that appears on the card.
        /// </summary>
        public string FirstName { get; }

        /// <summary>
        /// The last name that appears on the card.
        /// </summary>
        public string LastName { get; }

        /// <summary>
        /// The expiry month on the card.  <code>1</code> = January, <code>12</code> = December.
        /// </summary>
        public int? ExpMonth { get; }

        /// <summary>
        /// The expiry year on the card, for example <code>2018</code>.
        /// </summary>
        public int? ExpYear { get; }

        /// <summary>
        /// The long card number, may include spaces.  E.g. <code>4444 3333 2222 1111</code>.
        /// </summary>
        public string CardNumber { get; }

        /// <summary>
        /// The type of payment instrument, this should always be <code>Card</code>, responses from the producer would t ypically be <code>ObfuscatedCard</code> (where
        /// all but the last 4 digits are represented by asterisks).
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Card Verification Code, required for card not present transactions.
        /// </summary>
        public string Cvc { get; }

        /// <summary>
        /// Checks for equality based on all the attributes.
        /// </summary>
        public override bool Equals(object that)
        {
            return new EqualsBuilder<HceCard>(this, that)
                .With(m => m.FirstName)
                .With(m => m.LastName)
                .With(m => m.ExpMonth)
                .With(m => m.ExpYear)
                .With(m => m.CardNumber)
                .With(m => m.Type)
                .With(m => m.Cvc)
                .Equals();
        }

        /// <summary>
        /// Generates hash code based on all attributes.
        /// </summary>
        public override int GetHashCode()
        {
            return new HashCodeBuilder<HceCard>(this)
                .With(m => m.FirstName)
                .With(m => m.LastName)
                .With(m => m.ExpMonth)
                .With(m => m.ExpYear)
                .With(m => m.CardNumber)
                .With(m => m.Type)
                .With(m => m.Cvc)
                .HashCode;
        }

        /// <summary>
        /// Creaes human-readable version of the object that includes all attribute values.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return new ToStringBuilder<HceCard>(this)
                .Append(m => m.FirstName)
                .Append(m => m.LastName)
                .Append(m => m.ExpMonth)
                .Append(m => m.ExpYear)
                .Append(m => m.CardNumber)
                .Append(m => m.Type)
                .Append(m => m.Cvc)
                .ToString();
        }
    }
}
