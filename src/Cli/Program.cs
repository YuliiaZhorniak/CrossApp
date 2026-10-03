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


Console.WriteLine("=== Д 3: Переходи статусів (switch expression) ===");
var extraOrder = Order.Create("ORD-2026-88", "CUST-10");
extraOrder.AddLine("PROD-01", "Ковдра", 1200m, 1);

Console.WriteLine($"Початковий статус: {order.Status}");
extraOrder.Confirm();
Console.WriteLine($"Після Confirm(): {order.Status}");

extraOrder.Cancel();
Console.WriteLine($"Після Cancel(): {order.Status}");

try
{
    extraOrder.Confirm();
}
catch (Exception ex)
{
    Console.WriteLine($" {ex.GetType().Name}: {ex.Message}\n");
}


Console.WriteLine("=== Д1: Обробка ImportResult -> Domain ===");
var rawImport = new ImportResult<OrderDto>(
    Items: new List<OrderDto>
    {
        new("ORD-101", "CUST-01", DateTime.UtcNow, false, new List<OrderLineDto> { new("P1", "Товар 1", 100m, 2) }),
       
        new("ORD-102", "CUST-02", DateTime.UtcNow, false, new List<OrderLineDto> { new("P2", "Зламаний товар", -50m, 1) })
    },
    Errors: new List<string> { "[CSV-ERR] Помилка формату рядка 5 у файлі CSV" }
);

var domainResult = OrderDomainImporter.ProcessImport(rawImport);

Console.WriteLine($"Прийнято доменних замовлень: {domainResult.Items.Count}");
foreach (var validOrder in domainResult.Items)
{
    Console.WriteLine($"  • {validOrder}");
}

Console.WriteLine($"Загальний перелік помилок (імпорт + домен): {domainResult.ValidationErrors.Count}");
foreach (var err in domainResult.ValidationErrors)
{
    Console.WriteLine($"  ! {err}");
}
Console.WriteLine();


Console.WriteLine("=== Д 2: Інваріант для двох сутностей ===");
var existingOrders = new List<Order>
{
    Order.Create("ORD-1", "CUST-99"),
    Order.Create("ORD-2", "CUST-99"),
    Order.Create("ORD-3", "CUST-99")
};

try
{
    Console.WriteLine("Перевірка створення 4-го замовлення для клієнта CUST-99...");
    CustomerOrderPolicy.ValidateCanCreateOrder("CUST-99", existingOrders);
}
catch (Exception ex)
{
    Console.WriteLine($" {ex.GetType().Name}: {ex.Message}");
}