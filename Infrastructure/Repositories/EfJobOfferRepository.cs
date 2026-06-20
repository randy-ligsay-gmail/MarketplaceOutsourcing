namespace MarketplaceOutsourcing.Infrastructure.Repositories;

using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class EfJobOfferRepository : IJobOfferRepository
{
    private readonly MarketplaceDbContext _context;

    public EfJobOfferRepository(MarketplaceDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<JobOffer> GetAll()
    {
        return _context.JobOffers
            .AsNoTracking()
            .OrderBy(o => o.JobId)
            .ThenBy(o => o.Price)
            .ToList();
    }

    public IReadOnlyList<JobOffer> GetByJobId(Guid jobId)
    {
        return _context.JobOffers
            .Where(o => o.JobId == jobId)
            .OrderBy(o => o.Price)
            .ToList();
    }

    public JobOffer? GetById(Guid id)
    {
        return _context.JobOffers.FirstOrDefault(o => o.Id == id);
    }

    public void Add(JobOffer offer)
    {
        _context.JobOffers.Add(offer);
        _context.SaveChanges();
    }

    public void Remove(JobOffer offer)
    {
        _context.JobOffers.Remove(offer);
        _context.SaveChanges();
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
