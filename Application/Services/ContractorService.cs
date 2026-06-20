namespace MarketplaceOutsourcing.Application.Services;

using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;

public class ContractorService
{
    private readonly IContractorRepository _contractorRepository;

    public ContractorService(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public IReadOnlyList<Contractor> ListContractors() => _contractorRepository.GetAll();

    public IReadOnlyList<Contractor> SearchContractors(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Array.Empty<Contractor>();
        }

        var term = searchTerm.Trim();

        if (Guid.TryParse(term, out var contractorId))
        {
            var contractor = _contractorRepository.GetById(contractorId);
            return contractor is null ? Array.Empty<Contractor>() : new[] { contractor };
        }

        return _contractorRepository.SearchByName(term);
    }
}
