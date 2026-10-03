using Core.Dto;
namespace Core.Domain;

public sealed class Order
{
    private readonly List<OrderLine> _lines = [];

    public string Id { get; }
    public string CustomerId { get; }
    public DateTime CreatedAt { get; }
    public bool IsConfirmed { get; private set; } 

    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    public decimal Total => _lines.Sum(line => line.LineTotal);

    private Order(string id, string customerId, DateTime createdAt, bool isConfirmed)
    {
        Id = id;
        CustomerId = customerId;
        CreatedAt = createdAt;
        IsConfirmed = isConfirmed;
    }

    public static Order Create(string id, string customerId)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор замовлення не може бути порожнім.", nameof(id));

        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Ідентифікатор клієнта не може бути порожнім.", nameof(customerId));

        return new Order(
            id.Trim().ToUpperInvariant(),
            customerId.Trim(),
            DateTime.UtcNow,
            isConfirmed: false
        );
    }

    public void AddLine(string productId, string productName, decimal unitPrice, int quantity)
    {
        if (IsConfirmed)
            throw new InvalidOperationException($"Замовлення [{Id}] вже підтверджене, додавати нові рядки заборонено.");

        var line = new OrderLine(productId, productName, unitPrice, quantity);
        _lines.Add(line);
    }

    public void Confirm()
    {
        if (IsConfirmed)
            throw new InvalidOperationException($"Замовлення [{Id}] вже підтверджене.");

        if (_lines.Count == 0)
            throw new InvalidOperationException($"Неможливо підтвердити замовлення [{Id}]: воно не містить жодного товару.");

        IsConfirmed = true;
    }

    public OrderDto ToDto() => new(
        Id,
        CustomerId,
        CreatedAt,
        IsConfirmed,
        _lines.Select(l => new OrderLineDto(l.ProductId, l.ProductName, l.UnitPrice, l.Quantity)).ToList()
    );

    public static Order FromDto(OrderDto dto)
    {
        var order = Create(dto.Id, dto.CustomerId);

        if (dto.Lines != null)
        {
            foreach (var lineDto in dto.Lines)
            {
                order.AddLine(lineDto.ProductId, lineDto.ProductName, lineDto.UnitPrice, lineDto.Quantity);
            }
        }

        if (dto.IsConfirmed)
        {
            order.Confirm();
        }

        return order;
    }

    public override string ToString() =>
        $"Замовлення [{Id}] | Клієнт: {CustomerId} | Статус: {(IsConfirmed ? "Підтверджене" : "Чернетка")} | Загальна сума: {Total:C} | Рядків: {_lines.Count}";
}