using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcing
{
    public class CurrentState
    {
        public int QuantityOnHand { get; set; } = 10;

        public int GetQuantity()
        {
            Console.Write("Quantity: ");
            return Convert.ToInt32(Console.ReadLine());
        }

        public string GetAdjustmentReason()
        {
            Console.Write("Reason: ");
            return Console.ReadLine();
        }
    }
    public class WarehouseProduct
    {
        public string Sku { get; }
        private readonly IList<IEvent> _events = new List<IEvent>();
        private readonly CurrentState _currentState = new();
        public WarehouseProduct(string sku)
        {
            Sku = sku;
        }

        public IList<IEvent> GetEvents()
        {
            return _events;
        }

        public void AddEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case ProductShipped shipProduct:
                    Apply(shipProduct);
                    break;
                case ProductReceived receiveProduct:
                    Apply(receiveProduct);
                    break;
                case InventoryAdjusted inventoryAdjusted:
                    Apply(inventoryAdjusted);
                    break;

                default:
                    throw new InvalidOperationException("Unsupported event");
            }

            _events.Add(evnt);
        }

        public int GetQuantityOnHand()
        {
            return _currentState.QuantityOnHand;
        }

        public void ShipProduct(int qty)
        {
            if (qty > _currentState.QuantityOnHand)
            {
                throw new InvalidDomainException("We don't have enough quantity to ship.");
            }

            AddEvent(new ProductShipped(Sku, qty, DateTime.Now));
        }

        public void ReceiveProduct(int qty)
        {
            if (qty < 0)
            {
                throw new InvalidDomainException("Negative quantity can not be added.");
            }

            AddEvent(new ProductReceived(Sku, qty, DateTime.Now));
        }

        public void AdjustInventory(int qty, string reason)
        {
            if (qty + _currentState.QuantityOnHand < 0)
            {
                throw new InvalidDomainException("We don't have event");
            }

            AddEvent(new InventoryAdjusted(Sku, qty, reason ,DateTime.Now));
        }



        private void Apply(ProductShipped evnt)
        {
            _currentState.QuantityOnHand -= evnt.Quantity;
        }

        private void Apply(ProductReceived evnt)
        {
            _currentState.QuantityOnHand += evnt.Quantity;
        }
        
        private void Apply(InventoryAdjusted evnt)
        {
            _currentState.QuantityOnHand -= evnt.Quantity;
        }
    }
}

public class InvalidDomainException : Exception
{
    public InvalidDomainException(string message) 
        : base(message)
    {

    }
}
