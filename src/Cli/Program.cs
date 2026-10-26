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
using Core.Reports;

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


var sourceOrders = service.List().ToList();
var query = sourceOrders.Where(o => o.Total < 500m); 

Console.WriteLine($"Дефіцитних до змін: {query.Count()}");

var cheapOrder = service.CreateNewOrder("CUST-999");
sourceOrders.Add(cheapOrder);

Console.WriteLine($"Дефіцитних після змін: {query.Count()}"); 

var snapshot = query.ToList(); 

sourceOrders.Clear();
Console.WriteLine($"У знімку лишилось: {snapshot.Count}"); 


var items = service.List();
var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-top";

switch (command)
{
    case "report-top":
        Print(ReportService.TopByAmount(items, 5));
        break;
    case "report-expensive":
        Print(ReportService.ExpensiveOrders(items, 500m));
        break;
    case "report-groups":
        foreach (var g in ReportService.ByStatus(items))
        {
            Console.WriteLine($"Статус: {g.Key,-12} | Позицій: {g.Count,3} | Сумарно: {g.TotalAmount,10:C}");
        }
        break;
    case "report-sum":
        var t = ReportService.Totals(items);
        Console.WriteLine($"Всього замовлень: {t.Count}, Сумарно: {t.Total:C}, Середній чек: {t.Average:F2}, Максимальне: {t.Max:C}");
        break;
    default:
        Console.WriteLine("Доступні команди: report-top | report-expensive | report-groups | report-sum");
        break;
}

static void Print(IReadOnlyList<OrderReportLine> lines)
{
    foreach (var l in lines)
    {
        Console.WriteLine($" ID: {l.Id,-12} | Клієнт: {l.CustomerId,-12} | Сума: {l.Total,8:C} | Статус: {l.Status}");
    }
}