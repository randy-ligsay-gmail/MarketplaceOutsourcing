namespace MarketplaceOutsourcing.Api.Dtos;

public record LoginRequest(string Email, string Password);

public record RegisterCustomerRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName);

public record RegisterContractorRequest(
    string Email,
    string Password,
    string BusinessName,
    decimal Rating);

public record AuthResponse(
    string AccessToken,
    DateTime ExpiresAt,
    string Role,
    Guid? CustomerId,
    Guid? ContractorId);
