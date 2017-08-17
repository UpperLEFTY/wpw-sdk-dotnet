using System.Collections.Generic;
using System.Linq;
using ThriftService = Worldpay.Innovation.WPWithin.Rpc.Types.Service;

namespace Worldpay.Innovation.WPWithin.ThriftAdapters
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
                Prices = CollectionUtils.Copy(service.Prices, PriceAdapter.Create)
            };
        }

        public static Service Create(ThriftService service)
        {
            return new Service
            {
                Description = service.Description,
                Name = service.Name,
                Id = service.Id,
                Prices = PriceAdapter.Create(service.Prices)
            };
        }

        public static Dictionary<int, Service> Create(Dictionary<int, ThriftService> services)
        {
            return services.ToDictionary(pair => pair.Key, pair => Create(pair.Value));
        }

    }
}