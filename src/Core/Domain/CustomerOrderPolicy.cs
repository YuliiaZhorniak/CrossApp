namespace Core.Domain;

public static class CustomerOrderPolicy
{
    public const int MaxActiveOrdersPerCustomer = 3;

    public static void ValidateCanCreateOrder(string customerId, IEnumerable<Order> existingCustomerOrders)
    {
        int activeOrdersCount = existingCustomerOrders.Count(o => o.Status == OrderStatus.Draft || o.Status == OrderStatus.Confirmed);

        if (activeOrdersCount >= MaxActiveOrdersPerCustomer)
        {
            throw new InvalidOperationException(
                $"Клієнт [{customerId}] досяг ліміту відкритих замовлень ({activeOrdersCount}/{MaxActiveOrdersPerCustomer}). Створення нового замовлення заборонено.");
        }
    }
}