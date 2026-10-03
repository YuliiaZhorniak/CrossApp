namespace Core.Dto;

public sealed record ProductDto(
    string Id,
    string Name,
    decimal Price = 10,
    string? Note = null);