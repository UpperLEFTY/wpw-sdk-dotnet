using Newtonsoft.Json;
using System.Collections.Generic;

namespace Worldpay.Within.ThriftAdapters
{
    /// <summary>
    /// TODO: Fix KeyNotFoundExceptions by testing value before returning.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PspConfig
    {
        public const string PspNameProperty = "psp_name";
        public const string HtePublicKeyProperty = "hte_public_key";
        public const string HtePrivateKeyProperty = "hte_private_key";
        public const string ApiEndpointProperty = "api_endpoint";
        public const string MerchantClientKeyProperty = "merchant_client_key";
        public const string MerchantServiceKeyProperty = "merchant_service_key";

        public const string PspNameDefault = "worldpayonlinepayments";
        public const string ApiEndpointDefault = "https://api.worldpay.com/v1";

        private readonly Dictionary<string, string> _properties;

        public string this[string name]
        {
            get { return GetOrNull(name); }
            set { _properties[name] = value; }
        }

        public PspConfig()
        {
            _properties = new Dictionary<string, string>
            {
                // the defaults will be overwritten after read of consumer.json/producer.json
                {PspNameProperty, PspNameDefault},
                {ApiEndpointProperty, ApiEndpointDefault}
            };
        }

        [JsonProperty(PropertyName = PspNameProperty)]
        public string PspName
        {
            get { return GetOrNull(PspNameProperty); }
            set { _properties[PspNameProperty] = value; }
        }

        /// <summary>
        /// Syntactic sugar method to retrieve the named property from the underlying _properties attribute, or null if it doesn't exist.
        /// </summary>
        /// <param name="propertyName">Property to retrieve, must not be null.</param>
        /// <returns>The value of the property, or null if it doesn't exist (avoids <see cref="KeyNotFoundException"/>).</returns>
        private string GetOrNull(string propertyName)
        {
            string propertyValue;
            return _properties.TryGetValue(propertyName, out propertyValue) ? propertyValue : null;
        }

        [JsonProperty(PropertyName = HtePublicKeyProperty, NullValueHandling = NullValueHandling.Ignore)]
        public string HtePublicKey
        {
            get { return GetOrNull(HtePublicKeyProperty); }
            set { _properties[HtePublicKeyProperty] = value; }
        }

        [JsonProperty(PropertyName = HtePrivateKeyProperty, NullValueHandling = NullValueHandling.Ignore)]
        public string HtePrivateKey
        {
            get { return GetOrNull(HtePrivateKeyProperty); }
            set { _properties[HtePrivateKeyProperty] = value; }
        }

        [JsonProperty(PropertyName = MerchantClientKeyProperty, NullValueHandling = NullValueHandling.Ignore)]
        public string MerchantClientKey
        {
            get { return GetOrNull(MerchantClientKeyProperty); }
            set { _properties[MerchantClientKeyProperty] = value; }
        }

        [JsonProperty(PropertyName = MerchantServiceKeyProperty, NullValueHandling = NullValueHandling.Ignore)]
        public string MerchantServiceKey
        {
            get { return GetOrNull(MerchantServiceKeyProperty); }
            set { _properties[MerchantServiceKeyProperty] = value; }
        }

        [JsonProperty(PropertyName = ApiEndpointProperty)]
        public string ApiEndPoint
        {
            get { return GetOrNull(ApiEndpointProperty); }
            set { _properties[ApiEndpointProperty] = value; }
        }

        /// <summary>
        /// Exposes the configuration as the dictonary that the Thrift RPC Agent requires.
        /// </summary>
        /// <returns>A dictionary, never null, however contents depend on user's calls to other properties.</returns>
        internal Dictionary<string, string> ToThriftRepresentation()
        {
            return new Dictionary<string, string>(_properties);
        }
    }
}