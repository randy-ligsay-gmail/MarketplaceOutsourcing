using MarketplaceOutsourcing.Api.Dtos;
using MarketplaceOutsourcing.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceOutsourcing.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
    {
        var (success, result, errorMessage) = _authService.Login(request.Email, request.Password);

        if (!success || result is null)
        {
            return Unauthorized(errorMessage ?? "Invalid credentials.");
        }

        return Ok(ToResponse(result));
    }

    [AllowAnonymous]
    [HttpPost("register/customer")]
    public ActionResult<AuthResponse> RegisterCustomer([FromBody] RegisterCustomerRequest request)
    {
        var (success, result, errorMessage) = _authService.RegisterCustomer(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        if (!success || result is null)
        {
            return BadRequest(errorMessage ?? "Could not register customer.");
        }

        return CreatedAtAction(nameof(Login), ToResponse(result));
    }

    [AllowAnonymous]
    [HttpPost("register/contractor")]
    public ActionResult<AuthResponse> RegisterContractor([FromBody] RegisterContractorRequest request)
    {
        var (success, result, errorMessage) = _authService.RegisterContractor(
            request.Email,
            request.Password,
            request.BusinessName,
            request.Rating);

        if (!success || result is null)
        {
            return BadRequest(errorMessage ?? "Could not register contractor.");
        }

        return CreatedAtAction(nameof(Login), ToResponse(result));
    }

    private static AuthResponse ToResponse(AuthResult result) =>
        new(result.AccessToken, result.ExpiresAt, result.Role, result.CustomerId, result.ContractorId);
}
