using Core.Domain;

namespace Core.Reports;

public static class ReportService
{
        public static IReadOnlyList<OrderReportLine> ExpensiveOrders(IEnumerable<Order> source, decimal threshold) =>
        source.Where(o => o.Total > threshold)
              .OrderBy(o => o.Total)
              .ThenBy(o => o.Id, StringComparer.Ordinal)
              .Select(o => new OrderReportLine(o.Id, o.CustomerId, o.Total, o.Status.ToString()))
              .ToList();
    public static IReadOnlyList<OrderReportLine> TopByAmount(IEnumerable<Order> source, int n) =>
        source.OrderByDescending(o => o.Total)
              .ThenBy(o => o.Id, StringComparer.Ordinal)
              .Take(n)
              .Select(o => new OrderReportLine(o.Id, o.CustomerId, o.Total, o.Status.ToString()))
              .ToList();
    public static IReadOnlyList<OrderGroupSummary> ByStatus(IEnumerable<Order> source) =>
        source.GroupBy(o => o.Status.ToString(), StringComparer.OrdinalIgnoreCase)
              .Select(g => new OrderGroupSummary(g.Key, g.Count(), g.Sum(o => o.Total)))
              .OrderByDescending(s => s.TotalAmount)
              .ToList();

        public static (int Count, decimal Total, decimal Average, decimal Max) Totals(IEnumerable<Order> source)
    {
        var list = source.ToList();
        return list.Count == 0
            ? (0, 0m, 0m, 0m)
            : (list.Count, list.Sum(o => o.Total), list.Average(o => o.Total), list.Max(o => o.Total));
    }
}