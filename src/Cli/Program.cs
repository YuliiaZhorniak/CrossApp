using System;
using System.IO;
using System.Linq;
using System.Text;
using Core.Dto;
using Core.Import;
using Core.Domain;

Console.OutputEncoding = Encoding.UTF8;


Console.WriteLine("=== Сценарій 1: успіх ===");

Order order = Order.Create("ORD-2026-001", "CUST-1001");
Console.WriteLine(order);

order.AddLine("PROD-01", "Плед новорічний", 850m, 1);
order.AddLine("PROD-02", "Подушка для сну", 1650m, 2);

Console.WriteLine("\nПозиції у замовленні:");
foreach (var line in order.Lines)
{
    Console.WriteLine($"  • {line}");
}

order.Confirm();
Console.WriteLine($"\nСтан після підтвердження: {order}\n");


Console.WriteLine("=== Сценарій 2: порушення інваріантів  ===");

TryDo("Порожній ID замовлення", () => 
    Order.Create("", "CUST-1001"));

TryDo("Від'ємна ціна товару", () => 
{
    var tempOrder = Order.Create("ORD-TEST", "CUST-1001");
    tempOrder.AddLine("PROD-03", "Подушка", -1500m, 1);
});

TryDo("Нульова кількість товару", () => 
{
    var tempOrder = Order.Create("ORD-TEST", "CUST-1001");
    tempOrder.AddLine("PROD-03", "Дзеркало", 5000m, 0);
});

TryDo("Підтвердження порожнього замовлення", () => 
{
    var emptyOrder = Order.Create("ORD-EMPTY", "CUST-1001");
    emptyOrder.Confirm();
});

TryDo("Додавання товару в підтверджене замовлення", () => 
{
    order.AddLine("PROD-04", "Ковдра велика", 749m, 1);
});
static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($" {title}: виняток НЕ спрацював!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" {title}: {ex.GetType().Name} -> {ex.Message}");
    }
}