using BookAdventure.Entities;

namespace BookAdventure.Repositories.Interfaces;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<Customer?> GetByDniAsync(string dni);
    Task<IEnumerable<Customer>> SearchByDniAsync(string dniPattern);
    Task<IEnumerable<Customer>> SearchByNameAsync(string namePattern);
}
