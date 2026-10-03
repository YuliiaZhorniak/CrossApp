namespace Core.Dto;

public sealed record CustomerDto(
    string CustomerId,
    string Name,
    string Phone);