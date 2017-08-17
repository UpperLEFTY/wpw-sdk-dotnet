using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftHceCard = Worldpay.Innovation.WPWithin.Rpc.Types.HCECard;

namespace Worldpay.Innovation.WPWithin.ThriftAdapters
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
