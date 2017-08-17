using ThriftTotalPriceResponse = Worldpay.Innovation.WPWithin.Rpc.Types.TotalPriceResponse;

namespace Worldpay.Innovation.WPWithin.ThriftAdapters
{
    internal class TotalPriceResponseAdapter
    {
        public static TotalPriceResponse Create(ThriftTotalPriceResponse resp)
        {
            return new TotalPriceResponse()
            {
                ServerId = resp.ServerId,
                ClientId = resp.ClientId,
                MerchantClientKey = resp.MerchantClientKey,
                PaymentReferenceId = resp.PaymentReferenceId,
                PriceId = resp.PriceId,
                TotalPrice = resp.TotalPrice,
                UnitsToSupply = resp.UnitsToSupply,
            };
        }

        public static ThriftTotalPriceResponse Create(TotalPriceResponse request)
        {
            return new ThriftTotalPriceResponse
            {
                ServerId = request.ServerId,
                UnitsToSupply = request.UnitsToSupply,
                PriceId = request.PriceId,
                ClientId = request.ClientId,
                MerchantClientKey = request.MerchantClientKey,
                PaymentReferenceId = request.PaymentReferenceId,
                TotalPrice = request.TotalPrice
            };
        }
    }
}