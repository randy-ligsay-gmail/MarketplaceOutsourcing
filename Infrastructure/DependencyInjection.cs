using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Application.Services;
using MarketplaceOutsourcing.Infrastructure.Persistence;
using MarketplaceOutsourcing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

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

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CustomerService>();
        services.AddScoped<ContractorService>();
        services.AddScoped<JobService>();
        services.AddScoped<JobOfferService>();

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
