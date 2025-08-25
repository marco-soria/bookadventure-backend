using BookAdventure.Entities;
using BookAdventure.Persistence;
using BookAdventure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAdventure.Repositories.Implementation;

public class RentalOrderDetailRepository : BaseRepository<RentalOrderDetail>, IRentalOrderDetailRepository
{
    public RentalOrderDetailRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<RentalOrderDetail>> GetByRentalOrderIdAsync(int rentalOrderId)
    {
        return await _dbSet
            .Where(rod => rod.RentalOrderId == rentalOrderId)
            .Include(rod => rod.Book)
                .ThenInclude(b => b.Genre)
            .Include(rod => rod.RentalOrder)
                .ThenInclude(ro => ro.Customer)
            .ToListAsync();
    }

    public async Task<RentalOrderDetail?> GetByRentalOrderAndBookAsync(int rentalOrderId, int bookId)
    {
        return await _dbSet
            .Include(rod => rod.Book)
            .Include(rod => rod.RentalOrder)
            .FirstOrDefaultAsync(rod => rod.RentalOrderId == rentalOrderId && rod.BookId == bookId);
    }

    public async Task<IEnumerable<RentalOrderDetail>> GetOverdueDetailsAsync()
    {
        var currentDate = DateTime.UtcNow;
        return await _dbSet
            .Where(rod => rod.DueDate < currentDate && !rod.IsReturned)
            .Include(rod => rod.Book)
                .ThenInclude(b => b.Genre)
            .Include(rod => rod.RentalOrder)
                .ThenInclude(ro => ro.Customer)
            .ToListAsync();
    }

    public async Task<IEnumerable<RentalOrderDetail>> GetActiveRentalsForBookAsync(int bookId)
    {
        return await _dbSet
            .Where(rod => rod.BookId == bookId && !rod.IsReturned)
            .Include(rod => rod.RentalOrder)
                .ThenInclude(ro => ro.Customer)
            .ToListAsync();
    }
}
