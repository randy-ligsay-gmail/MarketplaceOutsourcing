using MarketplaceOutsourcing.Domain.Common;

namespace MarketplaceOutsourcing.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public Customer() { }

    public Customer(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}
