namespace MarketplaceOutsourcing.Application.Services;

using MarketplaceOutsourcing.Application.Caching;
using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILruCache _cache;

    public CustomerService(ICustomerRepository customerRepository, ILruCache cache)
    {
        _customerRepository = customerRepository;
        _cache = cache;
    }

    public IReadOnlyList<Customer> ListCustomers()
    {
        if (_cache.TryGet(CacheKeys.CustomersAll, out IReadOnlyList<Customer>? cached) && cached is not null)
        {
            return cached;
        }

        var customers = _customerRepository.GetAll();
        _cache.Set(CacheKeys.CustomersAll, customers);
        return customers;
    }

    public IReadOnlyList<Customer> SearchCustomers(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Array.Empty<Customer>();
        }

        var term = searchTerm.Trim();
        var cacheKey = CacheKeys.CustomersSearch(term);
        if (_cache.TryGet(cacheKey, out IReadOnlyList<Customer>? cached) && cached is not null)
        {
            return cached;
        }

        IReadOnlyList<Customer> results;
        if (Guid.TryParse(term, out var customerId))
        {
            var customer = _customerRepository.GetById(customerId);
            results = customer is null ? Array.Empty<Customer>() : new[] { customer };
        }
        else
        {
            results = _customerRepository.SearchByLastName(term);
        }

        _cache.Set(cacheKey, results);
        return results;
    }

    public (bool Success, Customer? Customer) CreateCustomer(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return (false, null);
        }

        var customer = new Customer(firstName.Trim(), lastName.Trim());
        _customerRepository.Add(customer);
        _cache.RemoveByPrefix(CacheKeys.CustomersPrefix);

        return (true, customer);
    }
}
