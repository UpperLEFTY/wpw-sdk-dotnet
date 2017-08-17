using System.Net;

using ThriftDevice = Worldpay.Innovation.WPWithin.Rpc.Types.Device;

namespace Worldpay.Innovation.WPWithin.ThriftAdapters
{
    internal class DeviceAdapter
    {
        public static ThriftDevice Create(Device device)
        {
            return new ThriftDevice
            {
                CurrencyCode = device.CurrencyCode,
                Description = device.Description,
                Ipv4Address = device.Ipv4Address.ToString(),
                Name = device.Name,
                Services = CollectionUtils.Copy(device.Services, ServiceAdapter.Create),
                Uid = device.Uid
            };
        }

        public static Device Create(ThriftDevice thriftDevice)
        {
            return new Device(thriftDevice.Uid)
            {
                CurrencyCode = thriftDevice.CurrencyCode,
                Description = thriftDevice.Description,
                Ipv4Address = IPAddress.Parse(thriftDevice.Ipv4Address),
                Name = thriftDevice.Name,
                Services = ServiceAdapter.Create(thriftDevice.Services)
            };
        }
    }
}
