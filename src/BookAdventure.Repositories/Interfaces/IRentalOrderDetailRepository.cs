using BookAdventure.Entities;

namespace BookAdventure.Repositories.Interfaces;

public interface IRentalOrderDetailRepository : IBaseRepository<RentalOrderDetail>
{
    Task<IEnumerable<RentalOrderDetail>> GetByRentalOrderIdAsync(int rentalOrderId);
    Task<RentalOrderDetail?> GetByRentalOrderAndBookAsync(int rentalOrderId, int bookId);
    Task<IEnumerable<RentalOrderDetail>> GetOverdueDetailsAsync();
    Task<IEnumerable<RentalOrderDetail>> GetActiveRentalsForBookAsync(int bookId);
}
