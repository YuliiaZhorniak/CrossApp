using System;
using System.IO;
using System.Linq;
using System.Text;
using Core.Dto;
using Core.Import;

Console.OutputEncoding = Encoding.UTF8;

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return;
}

string extension = Path.GetExtension(path).ToLowerInvariant();

if (path.Contains("mixed", StringComparison.OrdinalIgnoreCase))
{
    var mixedResult = MixedCsvImporter.Load(path);

    Console.WriteLine($"Завантажено товарів (P): {mixedResult.Products.Count}");
    foreach (var p in mixedResult.Products)
    {
        Console.WriteLine($" [Товар]    {p.Id,-6} | {p.Name,-25} | {p.Price,8:C}");
    }

    Console.WriteLine("\nЗавантажено замовників (C): " + mixedResult.Customers.Count);
    foreach (var c in mixedResult.Customers)
    {
        Console.WriteLine($" [Замовник] {c.Id,-6} | {c.Name,-25} | {c.Phone}");
    }

    if (mixedResult.Errors.Count > 0)
    {
        Console.WriteLine($"Пропущено рядків: {mixedResult.Errors.Count}");
        foreach (string e in mixedResult.Errors)
        {
            Console.WriteLine($" ! {e}");
        }
    }

    int totalMixed = mixedResult.Products.Count + mixedResult.Customers.Count + mixedResult.Errors.Count;
    int acceptedMixed = mixedResult.Products.Count + mixedResult.Customers.Count;
    int skippedMixed = mixedResult.Errors.Count;
    double errorRateMixed = totalMixed > 0 ? (double)skippedMixed / totalMixed * 100 : 0;

    Console.WriteLine($"Статистика: Усього: {totalMixed} | Прийнято: {acceptedMixed} | Пропущено: {skippedMixed} | Помилок: {errorRateMixed:F1}%");

    return;
}

ImportResult<ProductDto> result = extension switch
{
    ".csv" => ProductCsvImporter.Load(path),
    ".json" => ProductJsonImporter.Load(path),
    _ => new ImportResult<ProductDto>(new List<ProductDto>(), new[] { $"Непідтримуване розширення: {extension}" })
};

Console.WriteLine($"Завантажено записів: {result.Items.Count}");

foreach (ProductDto p in result.Items.Take(5))
{
    Console.WriteLine($" {p.Id,-6} | {p.Name,-30} | {p.Price,10:C}");
}

if (result.Errors.Count > 0)
{
    Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");
    foreach (string e in result.Errors)
    {
        Console.WriteLine($" ! {e}");
    }
}

int accepted = result.Items.Count;
int skipped = result.Errors.Count;
int total = accepted + skipped;
double errorRate = total > 0 ? (double)skipped / total * 100 : 0;

Console.WriteLine($"Статистика: Усього: {total} | Прийнято: {accepted} | Пропущено: {skipped} | Помилок: {errorRate:F1}%");