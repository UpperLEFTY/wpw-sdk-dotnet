using ThriftPaymentResponse = Worldpay.Innovation.WPWithin.Rpc.Types.PaymentResponse;

namespace Worldpay.Innovation.WPWithin.ThriftAdapters
{
    internal class PaymentResponseAdapter
    {
        public static PaymentResponse Create(ThriftPaymentResponse makePayment)
        {
            return new PaymentResponse(makePayment.ServerId, makePayment.ClientId, makePayment.TotalPaid,
                ServiceDeliveryTokenAdapter.Create(makePayment.ServiceDeliveryToken));
        }
    }
}