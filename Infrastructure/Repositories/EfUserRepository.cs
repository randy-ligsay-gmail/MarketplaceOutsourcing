namespace MarketplaceOutsourcing.Infrastructure.Repositories;

using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class EfUserRepository : IUserRepository
{
    private readonly MarketplaceDbContext _context;

    public EfUserRepository(MarketplaceDbContext context)
    {
        _context = context;
    }

    public User? GetByEmail(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return _context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Email == normalizedEmail);
    }

    public User? GetById(Guid id)
    {
        return _context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Id == id);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }
}
