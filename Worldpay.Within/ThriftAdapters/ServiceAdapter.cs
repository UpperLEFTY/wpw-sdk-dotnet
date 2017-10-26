using System.Collections.Generic;
using System.Linq;
using ThriftService = Worldpay.Within.Rpc.Types.Service;

namespace Worldpay.Within.ThriftAdapters
{
    internal class ServiceAdapter
    {
        public static ThriftService Create(Service service)
        {
            return new ThriftService()
            {
                Description = service.Description,
                Id = service.Id,
                Name = service.Name,
                Prices = CollectionUtils.Copy(service.Prices, PriceAdapter.Create),
                ServiceType = service.ServiceType
            };
        }

        public static Service Create(ThriftService service)
        {
            return new Service(service.Id)
            {
                Description = service.Description,
                Name = service.Name,
                Prices = PriceAdapter.Create(service.Prices),
                ServiceType = service.ServiceType
            };
        }

        public static Dictionary<int, Service> Create(Dictionary<int, ThriftService> services)
        {
            return services.ToDictionary(pair => pair.Key, pair => Create(pair.Value));
        }

    }
}