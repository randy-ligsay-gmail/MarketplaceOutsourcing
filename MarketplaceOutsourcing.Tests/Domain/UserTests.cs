using MarketplaceOutsourcing.Domain.Constants;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Domain.Exceptions;

namespace MarketplaceOutsourcing.Tests.Domain;

public class UserTests
{
    [Fact]
    public void CreateCustomer_NormalizesEmailAndSetsRole()
    {
        var customerId = Guid.NewGuid();

        var user = User.CreateCustomer("  John.Dela@Example.COM  ", "hash", customerId);

        Assert.Equal("john.dela@example.com", user.Email);
        Assert.Equal(AppRoles.Customer, user.Role);
        Assert.Equal(customerId, user.CustomerId);
        Assert.Null(user.ContractorId);
    }

    [Fact]
    public void CreateCustomer_WithEmptyCustomerId_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            User.CreateCustomer("test@example.com", "hash", Guid.Empty));

        Assert.Equal("Customer ID is required.", ex.Message);
    }

    [Fact]
    public void CreateAdmin_SetsAdminRole()
    {
        var user = User.CreateAdmin("admin@example.com", "hash");

        Assert.Equal(AppRoles.Admin, user.Role);
        Assert.Null(user.CustomerId);
        Assert.Null(user.ContractorId);
    }
}
