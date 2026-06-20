namespace MarketplaceOutsourcing.Application.Interfaces;

using MarketplaceOutsourcing.Domain.Entities;

public interface IJobOfferRepository
{
    IReadOnlyList<JobOffer> GetAll();
    IReadOnlyList<JobOffer> GetByJobId(Guid jobId);
    JobOffer? GetById(Guid id);
    void Add(JobOffer offer);
    void Remove(JobOffer offer);
    void SaveChanges();
}
