using Worldpay.Innovation.WPWithin.Utils;

namespace Worldpay.Innovation.WPWithin
{
    /// <summary>
    /// The response to a request to make a payment by a consumer to a producer. 
    /// </summary>
    public class PaymentResponse
    {
        public PaymentResponse(string serverId, string clientId, int? totalPaid, ServiceDeliveryToken serviceDeliveryToken)
        {
            ServerId = serverId;
            ClientId = clientId;
            TotalPaid = totalPaid ?? 0;
            ServiceDeliveryToken = serviceDeliveryToken;
        }

        /// <summary>
        /// The identity of the server that produced the response.
        /// </summary>
        public string ServerId { get; }

        /// <summary>
        /// The identity of the client has received the response.
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// The total amount of money paid for the service.
        /// </summary>
        public int TotalPaid { get; }

        /// <summary>
        /// A token that can be used by the consumer to initiate the delivery of the service from the producer. 
        /// </summary>
        public ServiceDeliveryToken ServiceDeliveryToken { get; }

        public override bool Equals(object that)
        {
            return new EqualsBuilder<PaymentResponse>(this, that)
                .With(m => m.ServerId)
                .With(m => m.ClientId)
                .With(m => m.TotalPaid)
                .With(m => m.ServiceDeliveryToken)
                .Equals();
        }

        public override int GetHashCode()
        {
            return new HashCodeBuilder<PaymentResponse>(this)
                .With(m => m.ServerId)
                .With(m => m.ClientId)
                .With(m => m.TotalPaid)
                .With(m => m.ServiceDeliveryToken)
                .HashCode;
        }

        public override string ToString()
        {
            return new ToStringBuilder<PaymentResponse>(this)
                .Append(m => m.ServerId)
                .Append(m => m.ClientId)
                .Append(m => m.TotalPaid)
                .Append(m => m.ServiceDeliveryToken)
                .ToString();
        }
    }
}
