using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Reports;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

var dataPath = "data/orders.json";
if (args.Length > 0)
{
    var fileArgIndex = Array.IndexOf(args, "--file");
    if (fileArgIndex >= 0 && args.Length > fileArgIndex + 1)
    {
        dataPath = args[fileArgIndex + 1];
    }
    else if (!args[0].StartsWith("-"))
    {
        dataPath = args[0];
    }
}

var store = new InMemoryOrderStore();
var service = new OrderService(store);

using var userCts = new CancellationTokenSource();
using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(userCts.Token, timeoutCts.Token);

Console.CancelKeyPress += (_, e) => 
{ 
    e.Cancel = true; 
    userCts.Cancel(); 
};

var progress = new Progress<int>(percent =>
{
    Console.Write($"\rПрогрес імпорту: {percent}%   ");
});

try
{
    var added = await service.ImportAsync(new JsonCatalogImporter(), dataPath, linkedCts.Token, progress);
    
    await Task.Delay(50);

    Console.WriteLine($"\nІмпортовано записів: {added}");
}
catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
{
    Console.WriteLine("\nОперацію перервано: перевищено час очікування (таймаут).");
}
catch (OperationCanceledException)
{
    Console.WriteLine("\nІмпорт скасовано.");
}
catch (DomainException ex)
{
    Console.WriteLine($"\nДані відхилено: {ex.Message}");
}
catch (FileNotFoundException)
{
    Console.WriteLine($"\nФайл не знайдено: {dataPath}");
}