using ThriftPaymentResponse = Worldpay.Within.Rpc.Types.PaymentResponse;

namespace Worldpay.Within.ThriftAdapters
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