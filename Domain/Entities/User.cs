using MarketplaceOutsourcing.Domain.Common;
using MarketplaceOutsourcing.Domain.Constants;
using MarketplaceOutsourcing.Domain.Exceptions;

namespace MarketplaceOutsourcing.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Role { get; private set; } = default!;
    public Guid? CustomerId { get; private set; }
    public Guid? ContractorId { get; private set; }

    private User() { }

    public static User CreateCustomer(string email, string passwordHash, Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("Customer ID is required.");
        }

        return new User
        {
            Email = NormalizeEmail(email),
            PasswordHash = passwordHash,
            Role = AppRoles.Customer,
            CustomerId = customerId
        };
    }

    public static User CreateContractor(string email, string passwordHash, Guid contractorId)
    {
        if (contractorId == Guid.Empty)
        {
            throw new DomainException("Contractor ID is required.");
        }

        return new User
        {
            Email = NormalizeEmail(email),
            PasswordHash = passwordHash,
            Role = AppRoles.Contractor,
            ContractorId = contractorId
        };
    }

    public static User CreateAdmin(string email, string passwordHash)
    {
        return new User
        {
            Email = NormalizeEmail(email),
            PasswordHash = passwordHash,
            Role = AppRoles.Admin
        };
    }

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email is required.");
        }

        return email.Trim().ToLowerInvariant();
    }
}
