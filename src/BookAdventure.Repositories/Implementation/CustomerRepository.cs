using BookAdventure.Entities;
using BookAdventure.Persistence;
using BookAdventure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAdventure.Repositories.Implementation;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByDniAsync(string dni)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.DNI == dni);
    }

    public async Task<IEnumerable<Customer>> SearchByDniAsync(string dniPattern)
    {
        return await _dbSet
            .Where(c => c.DNI.Contains(dniPattern))
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> SearchByNameAsync(string namePattern)
    {
        return await _dbSet
            .Where(c => c.FirstName.Contains(namePattern) || c.LastName.Contains(namePattern))
            .ToListAsync();
    }
}
