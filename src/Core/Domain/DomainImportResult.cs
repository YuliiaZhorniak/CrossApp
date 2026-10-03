namespace Core.Domain;

public record DomainImportResult<T>(
    IReadOnlyList<T> Items,
    IReadOnlyList<string> ValidationErrors
);