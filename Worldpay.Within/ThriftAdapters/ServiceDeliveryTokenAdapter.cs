using ThriftServiceDeliveryToken=Worldpay.Within.Rpc.Types.ServiceDeliveryToken;

namespace Worldpay.Within.ThriftAdapters
{
    internal class ServiceDeliveryTokenAdapter
    {
        public static ThriftServiceDeliveryToken Create(ServiceDeliveryToken serviceDeliveryToken)
        {
            return new ThriftServiceDeliveryToken
            {
                Key = serviceDeliveryToken.Key,
                RefundOnExpiry = serviceDeliveryToken.RefundOnExpiry,
                Signature = serviceDeliveryToken.Signature,
                Expiry = serviceDeliveryToken.Expiry,
                Issued = serviceDeliveryToken.Issued
            };
        }

        public static ServiceDeliveryToken Create(ThriftServiceDeliveryToken serviceDeliveryToken)
        {
            return new ServiceDeliveryToken
            {
                Key = serviceDeliveryToken.Key,
                RefundOnExpiry = serviceDeliveryToken.RefundOnExpiry,
                Expiry = serviceDeliveryToken.Expiry,
                Issued = serviceDeliveryToken.Issued,
                Signature = serviceDeliveryToken.Signature
            };
        }
    }
}