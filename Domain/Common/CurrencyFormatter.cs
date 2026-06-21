namespace MarketplaceOutsourcing.Domain.Common;

public static class CurrencyFormatter
{
    public static string Format(decimal amount) => $"₱{amount:N2}";
}
