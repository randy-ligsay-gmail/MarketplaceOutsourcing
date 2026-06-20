using MarketplaceOutsourcing.Domain.Common;

namespace MarketplaceOutsourcing.Domain.Entities;

public class Contractor : BaseEntity
{
    public string Name { get; private set; } = default!;
    public decimal Rating { get; private set; }

    private Contractor() { }

    public Contractor(string name, decimal rating)
    {
        Name = name.Trim();
        Rating = rating;
    }

    public string GetDetail() => $"{Id} | {Name} | rating {Rating:0.0}";
}
