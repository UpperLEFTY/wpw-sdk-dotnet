using System.Collections.Generic;
using System.Linq;
using Thrift.Collections;
using ThriftServiceMessage = Worldpay.Within.Rpc.Types.ServiceMessage;

namespace Worldpay.Within.ThriftAdapters
{
    internal class ServiceMessageAdapter
    {
        public static IEnumerable<ServiceMessage> Create(THashSet<ThriftServiceMessage> deviceDiscovery)
        {
            return deviceDiscovery.Select(Create);
        }

        public static ServiceMessage Create(ThriftServiceMessage sm)
        {
            return new ServiceMessage(sm.ServerId, sm.UrlPrefix, sm.PortNumber, sm.Hostname, sm.DeviceDescription);
        }
    }
}