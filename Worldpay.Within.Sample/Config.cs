using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Worldpay.Within.ThriftAdapters;

namespace Worldpay.Within.Sample
{
    /// <summary>
    /// Config class defining host, port, hceCard & pspConfig. The object is serializable to JSON.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    class Config
    {
        /// <summary>
        /// Port for RPC agent.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string host
        {
            get; set;
        }

        /// <summary>
        /// Host for RPC agent.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? port
        {
            get; set;
        }

        /// <summary>
        /// HCE card definition.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HceCard hceCard
        {
            get; set;
        }

        /// <summary>
        /// PSP configuration.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PspConfig pspConfig
        {
            get; set;
        }
    }
}
