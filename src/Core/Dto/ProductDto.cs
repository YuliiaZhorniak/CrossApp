namespace Core.Dto;

public sealed record ProductDto(
    string Id,
    string Name,
    decimal Price,
    string? Note = null);