namespace MarketplaceOutsourcing.Api.Dtos;

public record CustomerResponse(
    Guid Id,
    string Name,
    string LastName);

public record CreateCustomerRequest(
    string FirstName,
    string LastName);
