using MarketplaceOutsourcing.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace MarketplaceOutsourcing.Infrastructure.Caching;

public sealed class LruCache : ILruCache
{
    private readonly int _capacity;
    private readonly Dictionary<string, LinkedListNode<CacheEntry>> _entries = new();
    private readonly LinkedList<CacheEntry> _order = new();
    private readonly Lock _lock = new();

    public LruCache(IOptions<LruCacheSettings> settings)
    {
        _capacity = Math.Max(1, settings.Value.MaxEntries);
    }

    public bool TryGet<T>(string key, out T? value)
    {
        lock (_lock)
        {
            if (!_entries.TryGetValue(key, out var node))
            {
                value = default;
                return false;
            }

            _order.Remove(node);
            _order.AddFirst(node);

            if (node.Value.Value is T typedValue)
            {
                value = typedValue;
                return true;
            }

            value = default;
            return false;
        }
    }

    public void Set<T>(string key, T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        lock (_lock)
        {
            if (_entries.TryGetValue(key, out var existingNode))
            {
                existingNode.Value = new CacheEntry(key, value);
                _order.Remove(existingNode);
                _order.AddFirst(existingNode);
                return;
            }

            EvictIfFull();

            var node = new LinkedListNode<CacheEntry>(new CacheEntry(key, value));
            _order.AddFirst(node);
            _entries[key] = node;
        }
    }

    public void Remove(string key)
    {
        lock (_lock)
        {
            RemoveEntry(key);
        }
    }

    public void RemoveByPrefix(string prefix)
    {
        lock (_lock)
        {
            var keysToRemove = _entries.Keys
                .Where(key => key.StartsWith(prefix, StringComparison.Ordinal))
                .ToList();

            foreach (var key in keysToRemove)
            {
                RemoveEntry(key);
            }
        }
    }

    private void EvictIfFull()
    {
        while (_entries.Count >= _capacity)
        {
            var lruNode = _order.Last;
            if (lruNode is null)
            {
                break;
            }

            RemoveEntry(lruNode.Value.Key);
        }
    }

    private void RemoveEntry(string key)
    {
        if (!_entries.Remove(key, out var node))
        {
            return;
        }

        _order.Remove(node);
    }

    private sealed record CacheEntry(string Key, object Value);
}
