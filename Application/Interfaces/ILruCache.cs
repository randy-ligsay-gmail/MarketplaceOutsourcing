namespace MarketplaceOutsourcing.Application.Interfaces;

public interface ILruCache
{
    bool TryGet<T>(string key, out T? value);
    void Set<T>(string key, T value);
    void Remove(string key);
    void RemoveByPrefix(string prefix);
}
