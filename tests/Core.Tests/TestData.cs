using System.Collections.Generic;
using Core.Domain;

namespace Core.Tests;

public static class TestData
{
    public static IReadOnlyList<Order> Orders()
    {
        var order1 = Order.Create("ORD-0001", "CUST-A");
        order1.AddLine("PROD-1", "Подушка", 780m, 2); 

        var order2 = Order.Create("ORD-0002", "CUST-B");
        order2.AddLine("PROD-2","Дзеркало", 900m, 3); 

        return new List<Order> { order1, order2 };
    }
}