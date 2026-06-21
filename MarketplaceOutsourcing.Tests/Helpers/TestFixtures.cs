using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Domain.Enums;
using MarketplaceOutsourcing.Infrastructure.Caching;
using Microsoft.Extensions.Options;

namespace MarketplaceOutsourcing.Tests.Helpers;

internal static class TestFixtures
{
    public static Job CreateOpenJob(Guid? customerId = null) =>
        new(
            title: "Build API",
            description: "REST API with JWT",
            customerId: customerId ?? Guid.NewGuid(),
            startDate: DateTime.UtcNow.Date,
            dueDate: DateTime.UtcNow.Date.AddDays(14),
            budget: 5000m);

    public static Contractor CreateContractor() =>
        new("CloudNine DevOps LLC", 4.8m);

    public static LruCache CreateCache(int maxEntries = 16) =>
        new(Options.Create(new LruCacheSettings { MaxEntries = maxEntries }));
}
