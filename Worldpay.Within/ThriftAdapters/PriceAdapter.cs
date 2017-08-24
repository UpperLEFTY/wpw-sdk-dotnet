using System.Collections.Generic;
using System.Linq;
using Thrift.Collections;
using ThriftPrice = Worldpay.Innovation.WPWithin.Rpc.Types.Price;

namespace Worldpay.Innovation.WPWithin.ThriftAdapters
{
    internal class PriceAdapter
    {
        internal static ThriftPrice Create(Price price)
        {
            return new ThriftPrice
            {
                Description = price.Description,
                Id = price.Id,
                PricePerUnit = PricePerUnitAdapter.Create(price.PricePerUnit),
                UnitDescription = price.UnitDescription,
                UnitId = price.UnitId
            };
        }

        public static Dictionary<int, Price> Create(Dictionary<int, ThriftPrice> prices)
        {
            return prices.ToDictionary(pair => pair.Key, pair => Create(pair.Value));
        }

        private static Price Create(ThriftPrice prices)
        {
            return new Price(prices.Id ?? 0)
            {
                Description = prices.Description,
                PricePerUnit = PricePerUnitAdapter.Create(prices.PricePerUnit),
                UnitDescription = prices.UnitDescription,
                UnitId = prices.UnitId,
            };
        }

        public static IEnumerable<Price> Create(THashSet<ThriftPrice> getServicePrices)
        {
            return getServicePrices.Select(Create);
        }
    }
}