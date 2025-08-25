using BookAdventure.Entities;
using BookAdventure.Repositories.Interfaces;
using BookAdventure.Services.Interfaces;

namespace BookAdventure.Services.Implementation;

public class RentalOrderDetailService : IRentalOrderDetailService
{
    private readonly IRentalOrderDetailRepository _rentalOrderDetailRepository;
    private readonly IBookRepository _bookRepository;

    public RentalOrderDetailService(IRentalOrderDetailRepository rentalOrderDetailRepository, IBookRepository bookRepository)
    {
        _rentalOrderDetailRepository = rentalOrderDetailRepository;
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<RentalOrderDetail>> GetByRentalOrderIdAsync(int rentalOrderId)
    {
        return await _rentalOrderDetailRepository.GetByRentalOrderIdAsync(rentalOrderId);
    }

    public async Task<RentalOrderDetail?> GetByRentalOrderAndBookAsync(int rentalOrderId, int bookId)
    {
        return await _rentalOrderDetailRepository.GetByRentalOrderAndBookAsync(rentalOrderId, bookId);
    }

    public async Task<bool> ReturnBookAsync(int rentalOrderId, int bookId)
    {
        try
        {
            var detail = await _rentalOrderDetailRepository.GetByRentalOrderAndBookAsync(rentalOrderId, bookId);
            if (detail == null || detail.IsReturned)
                return false;

            detail.IsReturned = true;
            detail.ReturnDate = DateTime.UtcNow;

            // Restore book stock
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book != null)
            {
                book.Stock += detail.Quantity;
                book.IsAvailable = book.Stock > 0;
                await _bookRepository.UpdateAsync(book);
            }

            await _rentalOrderDetailRepository.UpdateAsync(detail);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<RentalOrderDetail>> GetOverdueDetailsAsync()
    {
        return await _rentalOrderDetailRepository.GetOverdueDetailsAsync();
    }

    // Base service implementations
    public async Task<IEnumerable<RentalOrderDetail>> GetAllAsync() => await _rentalOrderDetailRepository.GetAllAsync();
    public async Task<IEnumerable<RentalOrderDetail>> GetAllIncludingDeletedAsync() => await _rentalOrderDetailRepository.GetAllIncludingDeletedAsync();
    public async Task<RentalOrderDetail?> GetByIdAsync(int id) => await _rentalOrderDetailRepository.GetByIdAsync(id);
    public async Task<RentalOrderDetail?> GetByIdIncludingDeletedAsync(int id) => await _rentalOrderDetailRepository.GetByIdIncludingDeletedAsync(id);
    public async Task<IEnumerable<RentalOrderDetail>> FindAsync(System.Linq.Expressions.Expression<Func<RentalOrderDetail, bool>> predicate) => await _rentalOrderDetailRepository.FindAsync(predicate);
    public async Task<RentalOrderDetail> CreateAsync(RentalOrderDetail entity) => await _rentalOrderDetailRepository.CreateAsync(entity);
    public async Task<RentalOrderDetail?> UpdateAsync(RentalOrderDetail entity) => await _rentalOrderDetailRepository.UpdateAsync(entity);
    public async Task<bool> SoftDeleteAsync(int id) => await _rentalOrderDetailRepository.SoftDeleteAsync(id);
    public async Task<bool> RestoreAsync(int id) => await _rentalOrderDetailRepository.RestoreAsync(id);
    public async Task<bool> HardDeleteAsync(int id) => await _rentalOrderDetailRepository.HardDeleteAsync(id);
    public async Task<bool> ExistsAsync(int id) => await _rentalOrderDetailRepository.ExistsAsync(id);
    public async Task<int> CountAsync() => await _rentalOrderDetailRepository.CountAsync();
    public async Task<int> CountIncludingDeletedAsync() => await _rentalOrderDetailRepository.CountIncludingDeletedAsync();
}
