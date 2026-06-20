namespace MarketplaceOutsourcing.Infrastructure.Repositories;

using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Domain.Enums;
using MarketplaceOutsourcing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class EfJobRepository : IJobRepository
{
    private readonly MarketplaceDbContext _context;

    public EfJobRepository(MarketplaceDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Job> GetAll()
    {
        return _context.Jobs
            .AsNoTracking()
            .OrderBy(j => j.DueDate)
            .ToList();
    }

    public IReadOnlyList<Job> SearchOpenJobs(string searchTerm)
    {
        var term = searchTerm.Trim().ToLower();

        return _context.Jobs
            .AsNoTracking()
            .Where(j => j.Status == JobStatus.Open)
            .Where(j =>
                EF.Functions.ILike(j.Title, $"%{term}%") ||
                EF.Functions.ILike(j.Description, $"%{term}%"))
            .OrderBy(j => j.DueDate)
            .ToList();
    }

    public Job? GetById(Guid id)
    {
        return _context.Jobs.FirstOrDefault(j => j.Id == id);
    }

    public void Add(Job job)
    {
        _context.Jobs.Add(job);
        _context.SaveChanges();
    }

    public void Remove(Job job)
    {
        _context.Jobs.Remove(job);
        _context.SaveChanges();
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
