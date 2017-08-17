using System.Collections.Generic;
using System.Linq;
using Thrift.Collections;
using ThriftServiceMessage = Worldpay.Innovation.WPWithin.Rpc.Types.ServiceMessage;

namespace Worldpay.Innovation.WPWithin.ThriftAdapters
{
    internal class ServiceMessageAdapter
    {
        public static IEnumerable<ServiceMessage> Create(THashSet<Rpc.Types.ServiceMessage> deviceDiscovery)
        {
            return deviceDiscovery.Select(Create);
        }

        private static ServiceMessage Create(ThriftServiceMessage sm)
        {
            return new ServiceMessage(sm.ServerId, sm.UrlPrefix, sm.PortNumber, sm.Hostname, sm.DeviceDescription);
        }
    }
}