using EventSourcing;

var warehouseProductRepository = new WarehouseProductRepository();

var key = string.Empty;
while (key != "X")
{
    Console.WriteLine("R: Receive Inventory");
    Console.WriteLine("S: Ship Inventory");
    Console.WriteLine("A: Invetory Adjustment");
    Console.WriteLine("Q: Quantity On Hand");
    Console.WriteLine("E: Events");
    Console.Write("> ");
    key = Console.ReadLine()?.ToUpperInvariant();
    Console.WriteLine();

    Console.Write("SKU: ");
    string sku = Console.ReadLine();
    
    var warehouseProduct = warehouseProductRepository.Get(sku);
    CurrentState curState = new();

    switch (key)
    {
        case "R":
            int receiveInput = curState.GetQuantity();
            warehouseProduct.ReceiveProduct(receiveInput);
            Console.WriteLine($"{sku} Received: {receiveInput}");
            break;
        case "S":
            int shipInput = curState.GetQuantity();
            warehouseProduct.ShipProduct(shipInput);
            Console.WriteLine($"{sku} Shipped: {shipInput}");
            break;
        case "A":
            int adjustmentInput = curState.GetQuantity();
            string reason = curState.GetAdjustmentReason();
            warehouseProduct.AdjustInventory(adjustmentInput, reason);
            Console.WriteLine($"{sku} Adjusted: {adjustmentInput} Reason: {reason}");
            break;
        case "Q":
            var currentQuantityOnHand = warehouseProduct.GetQuantityOnHand();
            Console.WriteLine($"{sku} Quantity On Hand: {currentQuantityOnHand}");
            break;
        case "E":
            Console.WriteLine($"Events: {sku}");
            foreach (var evnt in warehouseProduct.GetEvents())
            {
                switch (evnt)
                {
                    case ProductShipped shipProduct:
                        Console.WriteLine($"{shipProduct.DateTime} {sku} Shipped: {shipProduct.Quantity}");
                        break;
                    case ProductReceived receiveProduct:
                        Console.WriteLine($"{receiveProduct.DateTime} {sku} Received: {receiveProduct.Quantity}");
                        break;
                    case InventoryAdjusted inventoryAdjusted:
                        Console.WriteLine($"{inventoryAdjusted.DateTime} {sku} Adjusted: {inventoryAdjusted.Quantity} {inventoryAdjusted.Reason}");
                        break;
                }
            }
            break;
    }

    warehouseProductRepository.Save(warehouseProduct);

    Console.ReadLine();
    Console.WriteLine();
}