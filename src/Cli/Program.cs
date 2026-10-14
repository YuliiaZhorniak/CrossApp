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
using Core.Events;

Console.OutputEncoding = Encoding.UTF8;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine("data", "orders.json");
IOrderStore store = useFile
    ? new FileOrderStore(dataPath)
    : new InMemoryOrderStore(SampleData.Orders());

var service = new OrderService(store);
var logPath = Path.Combine("logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

void OnOrderChanged(object? sender, OrderChangedEventArgs e)
{
    var line = $"{e.At:HH:mm:ss} {e.Kind,-10} {e.Id} {e.Delta,6:+0.00;-0.00;0.00} ➔ сума {e.TotalAfter:C}";
    
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

service.Changed += OnOrderChanged;
int changes = 0;
service.Changed += (_, _) => changes++;

Console.WriteLine($"Сховище: {store.GetType().Name}");

 Console.WriteLine("\n=== УСПІХ ===");
    var newOrder = service.CreateNewOrder("CUST-500");
    service.AddLineToOrder(newOrder.Id, "PROD-10", "Подушка для сну", 1650m, 1);
    service.ConfirmOrder(newOrder.Id);

    Console.WriteLine($"Створено та підтверджено замовлення: {newOrder.Id}");

    
Console.WriteLine("\n===  ПОДІЇ  ===");

var demoOrder = service.CreateNewOrder("CUST-777");

service.AddLineToOrder(demoOrder.Id, "PROD-99", "Дзеркало", 11500m, 1);

Console.WriteLine($"Подій отримано (до відписки): {changes}"); 

service.Changed -= OnOrderChanged;

service.AddLineToOrder(demoOrder.Id, "PROD-88", "Ковдра велика", 749m, 1);

Console.WriteLine($"Подій отримано (після відписки): {changes}"); 

Console.WriteLine("\n===  ВІДМОВА ===");
try
{
    service.AddLineToOrder("ORD-NONEXISTENT", "PROD-00", "Тест", 100m, 1);
}
catch (Exception ex)
{
    Console.WriteLine($"[ПЕРЕХОПЛЕНО ВИНЯТОК] {ex.GetType().Name} -> {ex.Message}");
}

Console.WriteLine("\n=== ФІЛЬТРАЦІЯ ===");

var expensiveOrders = service.FindAll(o => o.Total > 500m);

Console.WriteLine($"Знайдено замовлень з сумою > 500 грн: {expensiveOrders.Count}");
foreach (var ord in expensiveOrders)
{
    Console.WriteLine($" • {ord}");
}