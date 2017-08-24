using System.Collections.Generic;

namespace Worldpay.Innovation.WPWithin.ThriftAdapters
{
    /// <summary>
    /// TODO: Fix KeyNotFoundExceptions by testing value before returning.
    /// </summary>
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
                {PspNameProperty, PspNameDefault},
                {ApiEndpointProperty, ApiEndpointDefault}
            };
        }

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

        public string HtePublicKey
        {
            get { return GetOrNull(HtePublicKeyProperty); }
            set { _properties[HtePublicKeyProperty] = value; }
        }

        public string HtePrivateKey
        {
            get { return GetOrNull(HtePrivateKeyProperty); }
            set { _properties[HtePrivateKeyProperty] = value; }
        }

        public string MerchantClientKey
        {
            get { return GetOrNull(MerchantClientKeyProperty); }
            set { _properties[MerchantClientKeyProperty] = value; }
        }

        public string MerchantServiceKey
        {
            get { return GetOrNull(MerchantServiceKeyProperty); }
            set { _properties[MerchantServiceKeyProperty] = value; }
        }

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