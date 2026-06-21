namespace MarketplaceOutsourcing.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Role { get; }
    Guid? CustomerId { get; }
    Guid? ContractorId { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}
