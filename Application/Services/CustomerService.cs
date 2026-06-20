namespace MarketplaceOutsourcing.Application.Services;

using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public IReadOnlyList<Customer> ListCustomers()
    {
        return _customerRepository.GetAll();
    }

    public IReadOnlyList<Customer> SearchCustomers(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Array.Empty<Customer>();
        }

        var term = searchTerm.Trim();

        if (Guid.TryParse(term, out var customerId))
        {
            var customer = _customerRepository.GetById(customerId);
            return customer is null ? Array.Empty<Customer>() : new[] { customer };
        }

        return _customerRepository.SearchByLastName(term);
    }

    public (bool Success, Customer? Customer) CreateCustomer(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return (false, null);
        }

        var customer = new Customer(firstName.Trim(), lastName.Trim());
        _customerRepository.Add(customer);

        return (true, customer);
    }
}
