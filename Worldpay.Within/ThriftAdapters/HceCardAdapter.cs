using Worldpay.Within;
using ThriftHceCard = Worldpay.Within.Rpc.Types.HCECard;

namespace Worldpay.Within.ThriftAdapters
{
    internal class HceCardAdapter
    {
        public static ThriftHceCard Create(HceCard card)
        {
            return new ThriftHceCard()
            {
                CardNumber = card.CardNumber,
                ExpMonth = card.ExpMonth,
                Cvc = card.Cvc,
                ExpYear = card.ExpYear,
                FirstName = card.FirstName,
                LastName = card.LastName,
                Type = card.Type
            };
        }
    }
}
