using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IReadOnlyList<Order> Orders()
    {
        var order1 = Order.Create("ORD-2026-01", "CUST-101");
        order1.AddLine("PROD-01", "Дзеркало підлогове", 11500m, 1);
        order1.Confirm();

        var order2 = Order.Create("ORD-2026-02", "CUST-102");
        order2.AddLine("PROD-02", "Ковдра велика", 749m, 2);

        return [order1, order2];
    }
}