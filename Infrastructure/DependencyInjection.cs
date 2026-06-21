using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Application.Services;
using MarketplaceOutsourcing.Domain.Constants;
using MarketplaceOutsourcing.Infrastructure.Auth;
using MarketplaceOutsourcing.Infrastructure.Persistence;
using MarketplaceOutsourcing.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MarketplaceOutsourcing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddDbContext<MarketplaceDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ICustomerRepository, EfCustomerRepository>();
        services.AddScoped<IContractorRepository, EfContractorRepository>();
        services.AddScoped<IJobRepository, EfJobRepository>();
        services.AddScoped<IJobOfferRepository, EfJobOfferRepository>();
        services.AddScoped<IUserRepository, EfUserRepository>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, HttpCurrentUserService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                    ?? throw new InvalidOperationException("JWT settings are missing.");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("CustomerOnly", policy => policy.RequireRole(AppRoles.Customer));
            options.AddPolicy("ContractorOnly", policy => policy.RequireRole(AppRoles.Contractor));
            options.AddPolicy("AdminOnly", policy => policy.RequireRole(AppRoles.Admin));
        });

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CustomerService>();
        services.AddScoped<ContractorService>();
        services.AddScoped<JobService>();
        services.AddScoped<JobOfferService>();
        services.AddScoped<AuthService>();

        return services;
    }

    public static void ApplyMigrationsAndSeed(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
        context.Database.Migrate();
        DbSeeder.Seed(context);
    }
}
