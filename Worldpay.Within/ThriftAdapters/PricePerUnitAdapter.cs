﻿using ThriftPricePerUnit = Worldpay.Within.Rpc.Types.PricePerUnit;

namespace Worldpay.Within.ThriftAdapters
{
    /// <summary>
    ///     Thrift type adapter for <see cref="PricePerUnit" />.
    /// </summary>
    internal class PricePerUnitAdapter
    {
        internal static ThriftPricePerUnit Create(PricePerUnit pricePerUnit)
        {
            return new ThriftPricePerUnit()
            {
                Amount = pricePerUnit.Amount,
                CurrencyCode = pricePerUnit.CurrencyCode
            };
        }

        public static PricePerUnit Create(ThriftPricePerUnit pricePerUnit)
        {
            return new PricePerUnit(pricePerUnit.Amount ?? 0, pricePerUnit.CurrencyCode);
        }
    }
}