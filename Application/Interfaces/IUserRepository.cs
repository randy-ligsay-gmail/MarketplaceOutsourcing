using MarketplaceOutsourcing.Domain.Entities;

namespace MarketplaceOutsourcing.Application.Interfaces;

public interface IUserRepository
{
    User? GetByEmail(string email);
    User? GetById(Guid id);
    void Add(User user);
}
