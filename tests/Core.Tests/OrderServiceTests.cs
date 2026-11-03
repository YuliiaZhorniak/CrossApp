using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Core;
using Core.Domain;
using Core.Events; 
using Core.Services;
using Core.Storage;
using Core.Reports;

namespace Core.Tests;

public class OrderServiceTests
{
    [Fact]
    public void AddLine_WithNegativeQuantity_Throws_AndKeepsState()
    {
        var store = new InMemoryOrderStore();
        var service = new OrderService(store);
        var order = service.CreateNewOrder("CUST-1");

        Assert.Throws<ArgumentOutOfRangeException>(() => service.AddLineToOrder(order.Id, "P-1", "Товар", 150m, -1));
        
        Assert.Equal(0m, store.GetById(order.Id)!.Total);
    }

    [Fact]
    public void CreateNewOrder_RaisesChangedExactlyOnce()
    {
        var service = new OrderService(new InMemoryOrderStore());
        var events = new List<OrderChangedEventArgs>();
        service.Changed += (s, e) => events.Add(e);

        service.CreateNewOrder("CUST-2");

        Assert.Single(events);
        Assert.Equal(OrderChangeKind.Created, events[0].Kind);
    }

    [Theory]
    [InlineData(500, 2)]
    [InlineData(3000, 0)] 
    public void ExpensiveOrders_CountsOnlyAboveThreshold(decimal threshold, int expected)
    {
        var result = ReportService.ExpensiveOrders(TestData.Orders(), threshold);
        Assert.Equal(expected, result.Count);
    }

    [Fact]
    public void TopByAmount_IsDeterministic()
    {
        var top = ReportService.TopByAmount(TestData.Orders(), 1);
        Assert.Single(top);
        Assert.Equal("ORD-0002", top[0].Id); 
    }

    [Fact]
    public async Task ImportAsync_WhenAlreadyCancelled_Throws()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var service = new OrderService(new InMemoryOrderStore());
        var tempFile = Path.GetTempFileName();
        try
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => service.ImportAsync(new JsonCatalogImporter(), tempFile, cts.Token));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ImportAsync_WithInvalidData_ThrowsDomainException()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "[{\"Id\":\"\",\"CustomerId\":\"\"}]");

            var service = new OrderService(new InMemoryOrderStore());
            var importer = new JsonCatalogImporter();

            await Assert.ThrowsAnyAsync<Exception>(
                () => service.ImportAsync(importer, tempFile));
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddLine_ToConfirmedOrder_ThrowsDomainException()
    {
        var service = new OrderService(new InMemoryOrderStore());
        var order = service.CreateNewOrder("CUST-3");
        
        service.AddLineToOrder(order.Id, "P-1", "Тест", 100m, 1);
        service.ConfirmOrder(order.Id);

        Assert.Throws<InvalidOperationException>(() => 
            service.AddLineToOrder(order.Id, "P-99", "Товар", 100m, 1));
    }

    [Fact]
    public void Order_Total_CalculatesCorrectly_AfterAddingLines()
    {
        var order = Order.Create("ORD-TEST", "CUST-4");
        
        order.AddLine("P-1", "Товар 1", 100.50m, 2);
        order.AddLine("P-2", "Товар 2", 50.25m, 4);

        Assert.Equal(402.00m, order.Total);
    }

    [Fact]
    public async Task ImportAsync_WithTimeout_ThrowsOperationCanceledException()
    {
        using var timeoutSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));
        using var userSource = new CancellationTokenSource();
        
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, userSource.Token);

        await Task.Delay(15);

        var service = new OrderService(new InMemoryOrderStore());
        var importer = new JsonCatalogImporter();
        var tempFile = Path.GetTempFileName();

        try
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                service.ImportAsync(importer, tempFile, linkedCts.Token));

            Assert.True(timeoutSource.IsCancellationRequested);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}