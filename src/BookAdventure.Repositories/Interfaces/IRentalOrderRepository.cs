using BookAdventure.Entities;

namespace BookAdventure.Repositories.Interfaces;

public interface IRentalOrderRepository : IBaseRepository<RentalOrder>
{
    Task<IEnumerable<RentalOrder>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<RentalOrder>> GetOverdueRentalsAsync();
    Task<IEnumerable<RentalOrder>> GetActiveRentalsAsync();
    Task<RentalOrder?> GetWithDetailsAsync(int id);
}
