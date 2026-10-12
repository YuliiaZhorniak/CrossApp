using System;
using System.IO;
using System.Linq;
using System.Text;
using Core.Dto;
using Core.Import;
using Core.Domain;
using Core;
using Core.Abstractions;
using Core.Storage;
using Core.Services;

Console.OutputEncoding = Encoding.UTF8;
bool useFile = args.Contains("--file");
string dataPath = Path.Combine("data", "orders.json");IOrderStore store = useFile
    ? new FileOrderStore(dataPath)
    : new InMemoryOrderStore(SampleData.Orders());

var service = new OrderService(store);

Console.WriteLine($"Сховище: {store.GetType().Name}");

 Console.WriteLine("\n=== УСПІХ =");
    var newOrder = service.CreateNewOrder("CUST-500");
    service.AddLineToOrder(newOrder.Id, "PROD-10", "Подушка для сну", 1650m, 1);
    service.ConfirmOrder(newOrder.Id);

    Console.WriteLine($"Створено та підтверджено замовлення: {newOrder.Id}");

    Console.WriteLine("\nАктуальний список усіх замовлень у сховищі:");
    foreach (var order in service.All())
    {
        Console.WriteLine($"  • {order}");
        foreach (var line in order.Lines)
        {
            Console.WriteLine( $"      - {line.ProductName} | {line.Quantity} шт. x {line.UnitPrice} ₴ = {line.LineTotal} ₴");
        }
    }

Console.WriteLine("\n=== СЦЕНАРІЙ ВІДМОВИ ===");
try
{
    Console.WriteLine("Спроба підтвердити замовлення з неіснуючим ID ('ORD-NONEXISTENT')...");
    service.ConfirmOrder("ORD-NONEXISTENT");
}
catch (Exception ex)
{
    Console.WriteLine($"[ПЕРЕХОПЛЕНО ВИНЯТОК] {ex.GetType().Name} -> {ex.Message}[cite: 10, 14]");
}