using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Application.Services;
using MarketplaceOutsourcing.Domain.Constants;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Infrastructure.Caching;
using MarketplaceOutsourcing.Tests.Helpers;
using Moq;

namespace MarketplaceOutsourcing.Tests.Application;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ICustomerRepository> _customerRepository = new();
    private readonly Mock<IContractorRepository> _contractorRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();
    private readonly LruCache _cache = TestFixtures.CreateCache();

    private AuthService CreateService() =>
        new(
            _userRepository.Object,
            _customerRepository.Object,
            _contractorRepository.Object,
            _passwordHasher.Object,
            _jwtTokenService.Object,
            _cache);

    [Fact]
    public void Login_WithValidCredentials_ReturnsToken()
    {
        var user = User.CreateCustomer("john@example.com", "hashed", Guid.NewGuid());
        _userRepository.Setup(r => r.GetByEmail("john@example.com")).Returns(user);
        _passwordHasher.Setup(h => h.Verify("Password123!", "hashed")).Returns(true);
        _jwtTokenService.Setup(t => t.GenerateToken(user)).Returns(("token-123", DateTime.UtcNow.AddHours(1)));

        var service = CreateService();
        var (success, result, errorMessage) = service.Login("john@example.com", "Password123!");

        Assert.True(success);
        Assert.Null(errorMessage);
        Assert.NotNull(result);
        Assert.Equal("token-123", result!.AccessToken);
        Assert.Equal(AppRoles.Customer, result.Role);
    }

    [Fact]
    public void Login_WithInvalidPassword_ReturnsUnauthorizedMessage()
    {
        var user = User.CreateCustomer("john@example.com", "hashed", Guid.NewGuid());
        _userRepository.Setup(r => r.GetByEmail("john@example.com")).Returns(user);
        _passwordHasher.Setup(h => h.Verify("wrong", "hashed")).Returns(false);

        var service = CreateService();
        var (success, result, errorMessage) = service.Login("john@example.com", "wrong");

        Assert.False(success);
        Assert.Null(result);
        Assert.Equal("Invalid email or password.", errorMessage);
    }

    [Fact]
    public void RegisterCustomer_WithExistingEmail_ReturnsError()
    {
        _userRepository.Setup(r => r.GetByEmail("taken@example.com"))
            .Returns(User.CreateCustomer("taken@example.com", "hash", Guid.NewGuid()));

        var service = CreateService();
        var (success, result, errorMessage) = service.RegisterCustomer(
            "taken@example.com",
            "Password123!",
            "Jane",
            "Doe");

        Assert.False(success);
        Assert.Null(result);
        Assert.Equal("An account with this email already exists.", errorMessage);
        _customerRepository.Verify(r => r.Add(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public void RegisterContractor_WithShortPassword_ReturnsError()
    {
        var service = CreateService();
        var (success, result, errorMessage) = service.RegisterContractor(
            "new@example.com",
            "short",
            "Acme IT",
            4.5m);

        Assert.False(success);
        Assert.Null(result);
        Assert.Equal("Password must be at least 8 characters.", errorMessage);
    }
}
