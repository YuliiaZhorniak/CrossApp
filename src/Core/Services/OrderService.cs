using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class OrderService(IOrderStore store)
{
    private readonly IOrderStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public event EventHandler<OrderChangedEventArgs>? Changed;

    private void OnChanged(Order order, OrderChangeKind kind, decimal delta) =>
        Changed?.Invoke(this, new OrderChangedEventArgs(order.Id, kind, delta, order.Total, order.Lines.Count));

    public Order CreateNewOrder(string customerId)
    {
        var id = $"ORD-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var order = Order.Create(id, customerId);
        
        _store.Add(order);
        OnChanged(order, OrderChangeKind.Created, 0);
        
        return order;
    }

    public void AddLineToOrder(string orderId, string productId, string productName, decimal unitPrice, int quantity)
    {
        var order = _store.GetById(orderId) 
            ?? throw new InvalidOperationException($"Немає замовлення з id={orderId}.");
        
        order.AddLine(productId, productName, unitPrice, quantity);
        _store.Update(order);
        
        OnChanged(order, OrderChangeKind.LineAdded, unitPrice * quantity);
    }

    public void ConfirmOrder(string orderId)
    {
        var order = _store.GetById(orderId) 
            ?? throw new InvalidOperationException($"Немає замовлення з id={orderId}.");
        
        order.Confirm(); 
        _store.Update(order);
        
        OnChanged(order, OrderChangeKind.Confirmed, 0);
    }

    public IReadOnlyList<Order> List() => _store.List();
    
    public IReadOnlyList<Order> FindAll(Func<Order, bool> predicate) =>
        _store.List().Where(predicate).ToList();

    public async Task<int> ImportAsync(ICatalogImporter importer, string path, CancellationToken ct = default, IProgress<int>? progress = null)
    {
        var orders = (await importer.LoadAsync(path, ct)).ToList();
        var total = orders.Count;
        var added = 0;
        
        for (int i = 0; i < total; i++)
        {
            ct.ThrowIfCancellationRequested();
            var order = orders[i];
            
            if (_store.GetById(order.Id) is not null) continue;
            
            _store.Add(order);
            OnChanged(order, OrderChangeKind.Created, order.Total);
            added++;

            if (total > 0)
            {
                int percent = (i + 1) * 100 / total;
                progress?.Report(percent);
            }
        }
        
        return added;
    }

    public async Task<int> ImportStreamAsync(ICatalogImporter importer, string path, CancellationToken ct = default)
    {
        var added = 0;
        
        await foreach (var order in importer.LoadStreamAsync(path, ct))
        {
            ct.ThrowIfCancellationRequested();

            if (_store.GetById(order.Id) is not null)
            {
                continue;
            }

            _store.Add(order);
            OnChanged(order, OrderChangeKind.Created, order.Total);
            added++;
        }

        return added;
    }
}