using MarketplaceOutsourcing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceOutsourcing.Infrastructure.Persistence;

public class MarketplaceDbContext : DbContext
{
    public MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Contractor> Contractors => Set<Contractor>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobOffer> JobOffers => Set<JobOffer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(c => c.FirstName).HasMaxLength(100);
            entity.Property(c => c.LastName).HasMaxLength(100);
            entity.HasIndex(c => c.LastName);
        });

        modelBuilder.Entity<Contractor>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(200);
            entity.Property(c => c.Rating).HasPrecision(3, 1);
            entity.HasIndex(c => c.Name);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.Property(j => j.Title).HasMaxLength(200);
            entity.Property(j => j.Description).HasMaxLength(2000);
            entity.Property(j => j.Budget).HasPrecision(18, 2);
            entity.Property(j => j.Status).HasConversion<string>();
            entity.HasIndex(j => j.Status);
        });

        modelBuilder.Entity<JobOffer>(entity =>
        {
            entity.Property(o => o.Price).HasPrecision(18, 2);
            entity.Property(o => o.Status).HasConversion<string>();
            entity.HasIndex(o => o.JobId);
        });
    }
}
