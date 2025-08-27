using BookAdventure.Entities;
using BookAdventure.Persistence;
using BookAdventure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAdventure.Repositories.Implementation;

public class RentalOrderRepository : BaseRepository<RentalOrder>, IRentalOrderRepository
{
    public RentalOrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<RentalOrder>> GetByCustomerIdAsync(int customerId)
    {
        return await _dbSet
            .Where(ro => ro.CustomerId == customerId)
            .Include(ro => ro.Customer)
            .Include(ro => ro.RentalOrderDetails)
                .ThenInclude(rod => rod.Book)
                    .ThenInclude(b => b.Genre)
            .OrderByDescending(ro => ro.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<RentalOrder>> GetOverdueRentalsAsync()
    {
        var currentDate = DateTime.UtcNow;
        return await _dbSet
            .Where(ro => ro.OrderStatus == OrderStatus.Active && 
                        ro.RentalOrderDetails.Any(rod => rod.DueDate < currentDate && !rod.IsReturned))
            .Include(ro => ro.Customer)
            .Include(ro => ro.RentalOrderDetails)
                .ThenInclude(rod => rod.Book)
            .ToListAsync();
    }

    public async Task<IEnumerable<RentalOrder>> GetActiveRentalsAsync()
    {
        return await _dbSet
            .Where(ro => ro.OrderStatus == OrderStatus.Active)
            .Include(ro => ro.Customer)
            .Include(ro => ro.RentalOrderDetails)
                .ThenInclude(rod => rod.Book)
                    .ThenInclude(b => b.Genre)
            .OrderByDescending(ro => ro.OrderDate)
            .ToListAsync();
    }

    public async Task<RentalOrder?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(ro => ro.Customer)
            .Include(ro => ro.RentalOrderDetails)
                .ThenInclude(rod => rod.Book)
                    .ThenInclude(b => b.Genre)
            .FirstOrDefaultAsync(ro => ro.Id == id);
    }

    public async Task<RentalOrder?> GetWithDetailsIncludingDeletedAsync(int id)
    {
        return await _context.IncludeDeleted<RentalOrder>()
            .Include(ro => ro.Customer)
            .Include(ro => ro.RentalOrderDetails)
                .ThenInclude(rod => rod.Book)
                    .ThenInclude(b => b.Genre)
            .FirstOrDefaultAsync(ro => ro.Id == id);
    }

    public override async Task<RentalOrder?> GetByIdAsync(int id)
    {
        return await GetWithDetailsAsync(id);
    }

    public override async Task<IEnumerable<RentalOrder>> GetAllAsync()
    {
        return await _dbSet
            .Include(ro => ro.Customer)
            .Include(ro => ro.RentalOrderDetails)
                .ThenInclude(rod => rod.Book)
                    .ThenInclude(b => b.Genre)
            .OrderByDescending(ro => ro.OrderDate)
            .ToListAsync();
    }
}
