namespace Core.Domain;

public sealed class OrderLine
{
    public string ProductId { get; }
    public string ProductName { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; }

    public decimal LineTotal => UnitPrice * Quantity;

    public OrderLine(string productId, string productName, decimal unitPrice, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Ідентифікатор товару не може бути порожнім.", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Назва товару не може бути порожньою.", nameof(productName));

        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), unitPrice, "Ціна товару не може бути від'ємною.");

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "Кількість товару має бути більшою за нуль.");

        ProductId = productId.Trim();
        ProductName = productName.Trim();
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public override string ToString() => $"{ProductName} ({ProductId}) — {Quantity} шт. х {UnitPrice:C} = {LineTotal:C}";
}