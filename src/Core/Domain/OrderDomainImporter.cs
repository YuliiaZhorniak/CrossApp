using Core.Dto;

namespace Core.Domain;

public static class OrderDomainImporter
{
    public static DomainImportResult<Order> ProcessImport(ImportResult<OrderDto> rawImportResult)
    {
        var validOrders = new List<Order>();
        var errors = new List<string>(rawImportResult.Errors);

        foreach (var dto in rawImportResult.Items)
        {
            try
            {
                var order = Order.FromDto(dto);
                validOrders.Add(order);
            }
            catch (Exception ex)
            {
                errors.Add($"[D-ERR] Замовлення [{dto.Id}] відхилено доменною моделлю: {ex.Message}");
            }
        }

        return new DomainImportResult<Order>(validOrders, errors);
    }
}