using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MarketplaceOutsourcing.Application.Interfaces;

namespace MarketplaceOutsourcing.Infrastructure.Auth;

public class HttpCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public Guid? UserId => ParseGuid(ClaimTypes.NameIdentifier)
        ?? ParseGuid(JwtRegisteredClaimNames.Sub);

    public string? Role =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

    public Guid? CustomerId => ParseGuid("customer_id");

    public Guid? ContractorId => ParseGuid("contractor_id");

    public bool IsInRole(string role) =>
        _httpContextAccessor.HttpContext?.User?.IsInRole(role) == true;

    private Guid? ParseGuid(string claimType)
    {
        var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
