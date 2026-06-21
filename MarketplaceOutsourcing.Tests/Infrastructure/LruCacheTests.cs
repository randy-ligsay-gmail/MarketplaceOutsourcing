using MarketplaceOutsourcing.Infrastructure.Caching;
using Microsoft.Extensions.Options;

namespace MarketplaceOutsourcing.Tests.Infrastructure;

public class LruCacheTests
{
    [Fact]
    public void Set_WhenCapacityExceeded_EvictsLeastRecentlyUsedEntry()
    {
        var cache = new LruCache(Options.Create(new LruCacheSettings { MaxEntries = 2 }));

        cache.Set("a", 1);
        cache.Set("b", 2);
        cache.Set("c", 3);

        Assert.False(cache.TryGet<int>("a", out _));
        Assert.True(cache.TryGet("b", out int bValue));
        Assert.True(cache.TryGet("c", out int cValue));
        Assert.Equal(2, bValue);
        Assert.Equal(3, cValue);
    }

    [Fact]
    public void TryGet_MovesEntryToMostRecentlyUsed()
    {
        var cache = new LruCache(Options.Create(new LruCacheSettings { MaxEntries = 2 }));

        cache.Set("a", 1);
        cache.Set("b", 2);
        Assert.True(cache.TryGet("a", out int _));

        cache.Set("c", 3);

        Assert.True(cache.TryGet("a", out int aValue));
        Assert.False(cache.TryGet<int>("b", out _));
        Assert.True(cache.TryGet("c", out int cValue));
        Assert.Equal(1, aValue);
        Assert.Equal(3, cValue);
    }

    [Fact]
    public void RemoveByPrefix_RemovesMatchingKeysOnly()
    {
        var cache = new LruCache(Options.Create(new LruCacheSettings { MaxEntries = 8 }));

        cache.Set("jobs:all", "all-jobs");
        cache.Set("jobs:id:1", "job-1");
        cache.Set("customers:all", "all-customers");

        cache.RemoveByPrefix("jobs:");

        Assert.False(cache.TryGet<string>("jobs:all", out _));
        Assert.False(cache.TryGet<string>("jobs:id:1", out _));
        Assert.True(cache.TryGet("customers:all", out string? customers));
        Assert.Equal("all-customers", customers);
    }
}
