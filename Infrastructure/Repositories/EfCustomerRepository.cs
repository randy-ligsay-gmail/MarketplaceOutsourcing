namespace MarketplaceOutsourcing.Infrastructure.Repositories;

using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class EfCustomerRepository : ICustomerRepository
{
    private readonly MarketplaceDbContext _context;

    public EfCustomerRepository(MarketplaceDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Customer> GetAll()
    {
        return _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.LastName)
            .ToList();
    }

    public Customer? GetById(Guid id)
    {
        return _context.Customers
            .AsNoTracking()
            .FirstOrDefault(c => c.Id == id);
    }

    public IReadOnlyList<Customer> SearchByLastName(string partialLastName)
    {
        var term = partialLastName.Trim().ToLower();

        return _context.Customers
            .AsNoTracking()
            .Where(c => EF.Functions.ILike(c.LastName, $"%{term}%"))
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToList();
    }

    public void Add(Customer customer)
    {
        _context.Customers.Add(customer);
        _context.SaveChanges();
    }
}
