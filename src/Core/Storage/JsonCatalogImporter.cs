using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public sealed class JsonCatalogImporter : ICatalogImporter
{
    public async Task<IReadOnlyList<Order>> LoadAsync(string path, CancellationToken ct = default)
    {
        var orders = new List<Order>();
        await foreach (var order in LoadStreamAsync(path, ct))
        {
            orders.Add(order);
        }
        return orders;
    }

    public async IAsyncEnumerable<Order> LoadStreamAsync(
        string path, 
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Файл не знайдено: {path}");

        await using var stream = File.OpenRead(path);
        var orders = await JsonSerializer.DeserializeAsync<List<Order>>(stream, cancellationToken: ct);

        if (orders is not null)
        {
            foreach (var order in orders)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return order;
            }
        }
    }
}