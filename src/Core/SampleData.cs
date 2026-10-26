using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IReadOnlyList<Order> Orders()
    {
        var orders = new List<Order>();

        var o1 = Order.Create("ORD-2026-01", "CUST-101");
        o1.AddLine("PROD-01", "Дзеркало підлогове", 11500m, 1);
        o1.Confirm();
        orders.Add(o1);

        var o2 = Order.Create("ORD-2026-02", "CUST-102");
        o2.AddLine("PROD-02", "Офісне крісло", 7400m, 1);
        o2.Confirm();
        orders.Add(o2);

        var o3 = Order.Create("ORD-2026-03", "CUST-103");
        o3.AddLine("PROD-03", "Стіл письмовий", 9200m, 1);
        o3.Confirm();
        orders.Add(o3);

        var o4 = Order.Create("ORD-2026-04", "CUST-101");
        o4.AddLine("PROD-04", "Комод білий", 4500m, 2);
        o4.Confirm();
        orders.Add(o4);

        var o5 = Order.Create("ORD-2026-05", "CUST-104");
        o5.AddLine("PROD-05", "Диван двомісний", 18500m, 1);
        o5.Confirm();
        orders.Add(o5);

        var o6 = Order.Create("ORD-2026-06", "CUST-105");
        o6.AddLine("PROD-06", "Лампа настільна", 1200m, 2);
        orders.Add(o6);

        var o7 = Order.Create("ORD-2026-07", "CUST-102");
        o7.AddLine("PROD-07", "Килим для вітальні", 3400m, 1);
        o7.Confirm();
        orders.Add(o7);

        var o8 = Order.Create("ORD-2026-08", "CUST-106");
        o8.AddLine("PROD-08", "Штори оксамит зелений", 1800m, 2);
        orders.Add(o8);

        var o9 = Order.Create("ORD-2026-09", "CUST-107");
        o9.AddLine("PROD-09", "Комплект постільної білизни", 2100m, 1);
        o9.Confirm();
        orders.Add(o9);

        var o10 = Order.Create("ORD-2026-10", "CUST-103");
        o10.AddLine("PROD-10", "Матрац ортопедичний", 8900m, 1);
        o10.Confirm();
        orders.Add(o10);

        var o11 = Order.Create("ORD-2026-11", "CUST-108");
        o11.AddLine("PROD-11", "Свічка біла 4 шт", 250m, 4);
        orders.Add(o11);

        var o12 = Order.Create("ORD-2026-12", "CUST-109");
        o12.AddLine("PROD-12", "Набір рушників кухонних", 650m, 2);
        o12.Confirm();
        orders.Add(o12);

        var o13 = Order.Create("ORD-2026-13", "CUST-110");
        o13.AddLine("PROD-13", "Ароматична свічка Хвоя", 350m, 3);
        orders.Add(o13);

        var o14 = Order.Create("ORD-2026-14", "CUST-101");
        o14.AddLine("PROD-14", "Органайзер для косметики", 450m, 1);
        orders.Add(o14);

        var o15 = Order.Create("ORD-2026-15", "CUST-105");
        o15.AddLine("PROD-15", "Скринька для прикрас", 550m, 2);
        orders.Add(o15);

        var o16 = Order.Create("ORD-2026-16", "CUST-102");
        o16.AddLine("PROD-16", "Гном новорічний червоний", 399m, 1);
        o16.Confirm();
        orders.Add(o16);

        return orders;
    }
}