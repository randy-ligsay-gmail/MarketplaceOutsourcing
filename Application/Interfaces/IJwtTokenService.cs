using MarketplaceOutsourcing.Domain.Entities;

namespace MarketplaceOutsourcing.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
