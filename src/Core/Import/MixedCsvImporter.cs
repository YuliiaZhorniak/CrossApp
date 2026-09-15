using System.Globalization;
using System.Text;
using Core.Dto;

namespace Core.Import;

public static class MixedCsvImporter
{
    private const char Separator = ';';

    public record MixedImportResult(
        IReadOnlyList<ProductDto> Products,
        IReadOnlyList<CustomerDto> Customers,
        IReadOnlyList<string> Errors);

    public static MixedImportResult Load(string path)
    {
        var products = new List<ProductDto>();
        var customers = new List<CustomerDto>();
        var errors = new List<string>();

        string[] lines = File.ReadAllLines(path, Encoding.UTF8);

        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#') || line.StartsWith("type", StringComparison.OrdinalIgnoreCase))
                continue;

            switch (ParseLine(line))
            {
                case ParseProductOk p:
                    products.Add(p.Value);
                    break;
                case ParseCustomerOk c:
                    customers.Add(c.Value);
                    break;
                case ParseFailed failed:
                    errors.Add($"рядок {number}: {failed.Reason}");
                    break;
            }
        }

        return new MixedImportResult(products, customers, errors);
    }

    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        return parts switch
        {
            // P;id;name;price -> Товар
            ["P", var id, var name, var priceStr] when decimal.TryParse(priceStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal p) && p >= 0
                => new ParseProductOk(new ProductDto(id, name, p)),
            ["P", _, "", _] => new ParseFailed("назва товару порожня"),
            ["P", _, _, var priceStr] => new ParseFailed($"некоректна ціна товару: '{priceStr}'"),

            // C;id;name;phone -> Клієнт
            ["C", var id, var name, var phone] when !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(phone)
                => new ParseCustomerOk(new CustomerDto(id, name, phone)),
            ["C", _, "", _] => new ParseFailed("ім'я замовника порожнє"),

            // Невідомий префікс або некоректні колонки
            [var prefix, ..] when prefix != "P" && prefix != "C" => new ParseFailed($"невідомий префікс типу '{prefix}'"),
            _ => new ParseFailed($"некоректний формат рядка (колонок: {parts.Length})")
        };
    }

    private abstract record ParseOutcome;
    private sealed record ParseProductOk(ProductDto Value) : ParseOutcome;
    private sealed record ParseCustomerOk(CustomerDto Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}