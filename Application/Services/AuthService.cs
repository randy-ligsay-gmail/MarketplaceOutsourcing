using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;

namespace MarketplaceOutsourcing.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IContractorRepository _contractorRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUserRepository userRepository,
        ICustomerRepository customerRepository,
        IContractorRepository contractorRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _customerRepository = customerRepository;
        _contractorRepository = contractorRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public (bool Success, AuthResult? Result, string? ErrorMessage) Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return (false, null, "Email and password are required.");
        }

        var user = _userRepository.GetByEmail(email.Trim());
        if (user is null || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            return (false, null, "Invalid email or password.");
        }

        return (true, BuildAuthResult(user), null);
    }

    public (bool Success, AuthResult? Result, string? ErrorMessage) RegisterCustomer(
        string email,
        string password,
        string firstName,
        string lastName)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return (false, null, "Email and password are required.");
        }

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return (false, null, "First name and last name are required.");
        }

        if (password.Length < 8)
        {
            return (false, null, "Password must be at least 8 characters.");
        }

        if (_userRepository.GetByEmail(email.Trim()) is not null)
        {
            return (false, null, "An account with this email already exists.");
        }

        var customer = new Customer(firstName, lastName);
        _customerRepository.Add(customer);

        var user = User.CreateCustomer(email, _passwordHasher.Hash(password), customer.Id);
        _userRepository.Add(user);

        return (true, BuildAuthResult(user), null);
    }

    public (bool Success, AuthResult? Result, string? ErrorMessage) RegisterContractor(
        string email,
        string password,
        string businessName,
        decimal rating)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return (false, null, "Email and password are required.");
        }

        if (string.IsNullOrWhiteSpace(businessName))
        {
            return (false, null, "Business name is required.");
        }

        if (password.Length < 8)
        {
            return (false, null, "Password must be at least 8 characters.");
        }

        if (rating is < 0m or > 5m)
        {
            return (false, null, "Rating must be between 0 and 5.");
        }

        if (_userRepository.GetByEmail(email.Trim()) is not null)
        {
            return (false, null, "An account with this email already exists.");
        }

        var contractor = new Contractor(businessName, rating);
        _contractorRepository.Add(contractor);

        var user = User.CreateContractor(email, _passwordHasher.Hash(password), contractor.Id);
        _userRepository.Add(user);

        return (true, BuildAuthResult(user), null);
    }

    private AuthResult BuildAuthResult(User user)
    {
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

        return new AuthResult(
            token,
            expiresAt,
            user.Role,
            user.CustomerId,
            user.ContractorId);
    }
}

public record AuthResult(
    string AccessToken,
    DateTime ExpiresAt,
    string Role,
    Guid? CustomerId,
    Guid? ContractorId);
