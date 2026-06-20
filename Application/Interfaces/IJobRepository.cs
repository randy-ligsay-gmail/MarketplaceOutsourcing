namespace MarketplaceOutsourcing.Application.Interfaces;

using MarketplaceOutsourcing.Domain.Entities;

public interface IJobRepository
{
    IReadOnlyList<Job> GetAll();
    IReadOnlyList<Job> SearchOpenJobs(string searchTerm);
    Job? GetById(Guid id);
    void Add(Job job);
    void Remove(Job job);
    void SaveChanges();
}
