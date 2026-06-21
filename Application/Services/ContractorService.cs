namespace MarketplaceOutsourcing.Application.Services;

using MarketplaceOutsourcing.Application.Caching;
using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;

public class ContractorService
{
    private readonly IContractorRepository _contractorRepository;
    private readonly ILruCache _cache;

    public ContractorService(IContractorRepository contractorRepository, ILruCache cache)
    {
        _contractorRepository = contractorRepository;
        _cache = cache;
    }

    public IReadOnlyList<Contractor> ListContractors()
    {
        if (_cache.TryGet(CacheKeys.ContractorsAll, out IReadOnlyList<Contractor>? cached) && cached is not null)
        {
            return cached;
        }

        var contractors = _contractorRepository.GetAll();
        _cache.Set(CacheKeys.ContractorsAll, contractors);
        return contractors;
    }

    public IReadOnlyList<Contractor> SearchContractors(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Array.Empty<Contractor>();
        }

        var term = searchTerm.Trim();
        var cacheKey = CacheKeys.ContractorsSearch(term);
        if (_cache.TryGet(cacheKey, out IReadOnlyList<Contractor>? cached) && cached is not null)
        {
            return cached;
        }

        IReadOnlyList<Contractor> results;
        if (Guid.TryParse(term, out var contractorId))
        {
            var contractor = _contractorRepository.GetById(contractorId);
            results = contractor is null ? Array.Empty<Contractor>() : new[] { contractor };
        }
        else
        {
            results = _contractorRepository.SearchByName(term);
        }

        _cache.Set(cacheKey, results);
        return results;
    }
}
