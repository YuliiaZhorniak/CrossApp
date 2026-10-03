namespace Core.Dto;

public record OrderLineDto(
    string ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity
);

public record OrderDto(
    string Id,
    string CustomerId,
    DateTime CreatedAt,
    bool IsConfirmed,
    List<OrderLineDto>? Lines
);