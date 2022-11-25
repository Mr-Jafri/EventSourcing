using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcing
{
    public class WarehouseProductRepository
    {
        private readonly Dictionary<string, IList<IEvent>> _inMemoryStreams = new();

        public WarehouseProduct Get(string Sku)
        {
            var warehouseProduct = new WarehouseProduct(Sku);

            if (_inMemoryStreams.ContainsKey(Sku))
            {
                foreach (var evnt in _inMemoryStreams[Sku])
                {
                    warehouseProduct.AddEvent(evnt);
                }
            }

            return warehouseProduct;
        }

        public void Save(WarehouseProduct warehouseProduct)
        {
            _inMemoryStreams[warehouseProduct.Sku] = warehouseProduct.GetEvents();
        }
    }
}
