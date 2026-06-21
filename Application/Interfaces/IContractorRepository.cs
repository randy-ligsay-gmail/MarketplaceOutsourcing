namespace MarketplaceOutsourcing.Application.Interfaces;

using MarketplaceOutsourcing.Domain.Entities;

public interface IContractorRepository
{
    IReadOnlyList<Contractor> GetAll();
    Contractor? GetById(Guid id);
    IReadOnlyList<Contractor> SearchByName(string partialName);
    void Add(Contractor contractor);
}
