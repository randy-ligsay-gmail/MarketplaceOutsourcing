namespace MarketplaceOutsourcing.Infrastructure.Caching;

public class LruCacheSettings
{
    public const string SectionName = "LruCache";

    public int MaxEntries { get; set; } = 256;
}
