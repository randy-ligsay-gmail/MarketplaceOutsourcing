namespace MarketplaceOutsourcing.Application.Caching;

public static class CacheKeys
{
    public const string JobsPrefix = "jobs:";
    public const string JobOffersPrefix = "joboffers:";
    public const string CustomersPrefix = "customers:";
    public const string ContractorsPrefix = "contractors:";

    public static string JobsAll => $"{JobsPrefix}all";

    public static string JobById(Guid id) => $"{JobsPrefix}id:{id:D}";

    public static string JobsSearch(string searchTerm) =>
        $"{JobsPrefix}search:{searchTerm.Trim().ToLowerInvariant()}";

    public static string JobOffersAll => $"{JobOffersPrefix}all";

    public static string JobOfferById(Guid id) => $"{JobOffersPrefix}id:{id:D}";

    public static string JobOffersByJob(Guid jobId) => $"{JobOffersPrefix}job:{jobId:D}";

    public static string CustomersAll => $"{CustomersPrefix}all";

    public static string CustomersSearch(string searchTerm) =>
        $"{CustomersPrefix}search:{searchTerm.Trim().ToLowerInvariant()}";

    public static string ContractorsAll => $"{ContractorsPrefix}all";

    public static string ContractorsSearch(string searchTerm) =>
        $"{ContractorsPrefix}search:{searchTerm.Trim().ToLowerInvariant()}";
}
