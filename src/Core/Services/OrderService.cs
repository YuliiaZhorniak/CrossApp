using Core.Abstractions;
using Core.Domain;

namespace Core.Services;

public sealed class OrderService(IOrderStore store)
{
    private readonly IOrderStore _store = store ?? throw new ArgumentNullException(nameof(store));

   
    public Order CreateNewOrder(string customerId)
    {
        string orderId = "ORD-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var order = Order.Create(orderId, customerId);
        _store.Add(order);
        return order;
    }

    
    public void AddLineToOrder(string orderId, string productId, string productName, decimal unitPrice, int quantity)
    {
        var order = _store.GetById(orderId) 
            ?? throw new InvalidOperationException($"Немає замовлення з id={orderId}.");

        order.AddLine(productId, productName, unitPrice, quantity);
        _store.Update(order);
    }

    public void ConfirmOrder(string orderId)
    {
        var order = _store.GetById(orderId) 
            ?? throw new InvalidOperationException($"Немає замовлення з id={orderId}.");

        order.Confirm();
        _store.Update(order);
    }

    public IReadOnlyList<Order> All() => _store.List();

    public Order? Find(string id) => _store.GetById(id);
}