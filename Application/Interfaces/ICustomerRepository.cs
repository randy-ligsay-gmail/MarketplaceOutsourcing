namespace MarketplaceOutsourcing.Application.Interfaces;

using MarketplaceOutsourcing.Domain.Entities;

public interface ICustomerRepository
{
    IReadOnlyList<Customer> GetAll();
    Customer? GetById(Guid id);
    IReadOnlyList<Customer> SearchByLastName(string partialLastName);
    void Add(Customer customer);
}
