namespace MarketplaceOutsourcing.Infrastructure.Repositories;

using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class EfContractorRepository : IContractorRepository
{
    private readonly MarketplaceDbContext _context;

    public EfContractorRepository(MarketplaceDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Contractor> GetAll()
    {
        return _context.Contractors
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToList();
    }

    public Contractor? GetById(Guid id)
    {
        return _context.Contractors
            .AsNoTracking()
            .FirstOrDefault(c => c.Id == id);
    }

    public IReadOnlyList<Contractor> SearchByName(string partialName)
    {
        var term = partialName.Trim().ToLower();

        return _context.Contractors
            .AsNoTracking()
            .Where(c => EF.Functions.ILike(c.Name, $"%{term}%"))
            .OrderBy(c => c.Name)
            .ToList();
    }

    public void Add(Contractor contractor)
    {
        _context.Contractors.Add(contractor);
        _context.SaveChanges();
    }
}
