using AutoMapper;
using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Repositories.Interfaces;
using BookAdventure.Repositories.Utils;
using BookAdventure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAdventure.Services.Implementation;

public class RentalOrderService : IRentalOrderService
{
    private readonly IRentalOrderRepository _rentalOrderRepository;
    private readonly IRentalOrderDetailRepository _rentalOrderDetailRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public RentalOrderService(
        IRentalOrderRepository rentalOrderRepository,
        IRentalOrderDetailRepository rentalOrderDetailRepository,
        IBookRepository bookRepository,
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _rentalOrderRepository = rentalOrderRepository;
        _rentalOrderDetailRepository = rentalOrderDetailRepository;
        _bookRepository = bookRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetAsync(PaginationDto pagination)
    {
        try
        {
            var rentalOrders = await _rentalOrderRepository.Query()
                .Include(ro => ro.Customer)
                .Include(ro => ro.RentalOrderDetails)
                    .ThenInclude(rod => rod.Book)
                        .ThenInclude(b => b.Genre)
                .OrderByDescending(ro => ro.OrderDate)
                .Paginate(pagination)
                .ToListAsync();

            var totalRecords = await _rentalOrderRepository.CountAsync();
            var response = _mapper.Map<ICollection<RentalOrderResponseDto>>(rentalOrders);

            return new BaseResponseGeneric<ICollection<RentalOrderResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<ICollection<RentalOrderResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving rental orders: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<RentalOrderResponseDto>> GetAsync(int id)
    {
        try
        {
            var rentalOrder = await _rentalOrderRepository.GetWithDetailsAsync(id);

            if (rentalOrder == null)
            {
                return new BaseResponseGeneric<RentalOrderResponseDto>
                {
                    Success = false,
                    ErrorMessage = "Rental order not found"
                };
            }

            var response = _mapper.Map<RentalOrderResponseDto>(rentalOrder);

            return new BaseResponseGeneric<RentalOrderResponseDto>
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<RentalOrderResponseDto>
            {
                Success = false,
                ErrorMessage = $"Error retrieving rental order: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<int>> CreateRentalOrderAsync(RentalOrderRequestDto request)
    {
        try
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
            {
                return new BaseResponseGeneric<int>
                {
                    Success = false,
                    ErrorMessage = "Customer not found"
                };
            }

            // Validate books availability
            foreach (var detail in request.Details)
            {
                var book = await _bookRepository.GetByIdAsync(detail.BookId);
                if (book == null)
                {
                    return new BaseResponseGeneric<int>
                    {
                        Success = false,
                        ErrorMessage = $"Book with ID {detail.BookId} not found"
                    };
                }

                if (book.Stock < detail.Quantity)
                {
                    return new BaseResponseGeneric<int>
                    {
                        Success = false,
                        ErrorMessage = $"Insufficient stock for book '{book.Title}'. Available: {book.Stock}, Requested: {detail.Quantity}"
                    };
                }
            }

            // Create rental order
            var rentalOrder = new RentalOrder
            {
                CustomerId = request.CustomerId,
                OrderDate = DateTime.UtcNow,
                DueDate = request.DueDate,
                OrderNumber = GenerateOrderNumber(),
                OrderStatus = OrderStatus.Active,
                Notes = request.Notes,
                RentalOrderDetails = new List<RentalOrderDetail>()
            };

            // Create rental order details
            foreach (var detail in request.Details)
            {
                var book = await _bookRepository.GetByIdAsync(detail.BookId);
                var rentalOrderDetail = new RentalOrderDetail
                {
                    BookId = detail.BookId,
                    Quantity = detail.Quantity,
                    RentalDays = detail.RentalDays,
                    DueDate = DateTime.UtcNow.AddDays(detail.RentalDays),
                    Notes = detail.Notes,
                    IsReturned = false
                };

                rentalOrder.RentalOrderDetails.Add(rentalOrderDetail);

                // Update book stock and availability
                book!.Stock -= detail.Quantity;
                book.IsAvailable = book.Stock > 0;
                await _bookRepository.UpdateAsync(book);
            }

            var createdRentalOrder = await _rentalOrderRepository.CreateAsync(rentalOrder);

            return new BaseResponseGeneric<int>
            {
                Success = true,
                Data = createdRentalOrder.Id
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<int>
            {
                Success = false,
                ErrorMessage = $"Error creating rental order: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> UpdateAsync(int id, RentalOrderRequestDto request)
    {
        try
        {
            var existingRentalOrder = await _rentalOrderRepository.GetWithDetailsAsync(id);
            if (existingRentalOrder == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Rental order not found"
                };
            }

            // Only allow updating certain fields for active orders
            if (existingRentalOrder.OrderStatus == OrderStatus.Active)
            {
                existingRentalOrder.DueDate = request.DueDate;
                existingRentalOrder.Notes = request.Notes;
                
                await _rentalOrderRepository.UpdateAsync(existingRentalOrder);

                return new BaseResponse { Success = true };
            }

            return new BaseResponse
            {
                Success = false,
                ErrorMessage = "Cannot update completed or cancelled rental orders"
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error updating rental order: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> DeleteAsync(int id)
    {
        try
        {
            var rentalOrder = await _rentalOrderRepository.GetWithDetailsAsync(id);
            if (rentalOrder == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Rental order not found"
                };
            }

            // Only allow cancelling active orders
            if (rentalOrder.OrderStatus == OrderStatus.Active)
            {
                // Restore book stock for cancelled orders
                foreach (var detail in rentalOrder.RentalOrderDetails.Where(d => !d.IsReturned))
                {
                    var book = await _bookRepository.GetByIdAsync(detail.BookId);
                    if (book != null)
                    {
                        book.Stock += detail.Quantity;
                        book.IsAvailable = book.Stock > 0;
                        await _bookRepository.UpdateAsync(book);
                    }
                }

                rentalOrder.OrderStatus = OrderStatus.Cancelled;
                await _rentalOrderRepository.UpdateAsync(rentalOrder);

                return new BaseResponse { Success = true };
            }

            return new BaseResponse
            {
                Success = false,
                ErrorMessage = "Cannot cancel completed rental orders"
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error cancelling rental order: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetByCustomerAsync(int customerId, PaginationDto pagination)
    {
        try
        {
            var rentalOrders = await _rentalOrderRepository.Query()
                .Where(ro => ro.CustomerId == customerId)
                .Include(ro => ro.Customer)
                .Include(ro => ro.RentalOrderDetails)
                    .ThenInclude(rod => rod.Book)
                        .ThenInclude(b => b.Genre)
                .OrderByDescending(ro => ro.OrderDate)
                .Paginate(pagination)
                .ToListAsync();

            var totalRecords = await _rentalOrderRepository.Query()
                .Where(ro => ro.CustomerId == customerId)
                .CountAsync();

            var response = _mapper.Map<ICollection<RentalOrderResponseDto>>(rentalOrders);

            return new BaseResponseGeneric<ICollection<RentalOrderResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<ICollection<RentalOrderResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving customer rental orders: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> ReturnBooksAsync(int rentalOrderId, List<int> bookIds)
    {
        try
        {
            var rentalOrder = await _rentalOrderRepository.GetWithDetailsAsync(rentalOrderId);
            if (rentalOrder == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Rental order not found"
                };
            }

            foreach (var bookId in bookIds)
            {
                var detail = rentalOrder.RentalOrderDetails.FirstOrDefault(d => d.BookId == bookId && !d.IsReturned);
                if (detail != null)
                {
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
                }
            }

            // Check if all books are returned
            if (rentalOrder.RentalOrderDetails.All(d => d.IsReturned))
            {
                rentalOrder.OrderStatus = OrderStatus.Returned;
                rentalOrder.ReturnDate = DateTime.UtcNow;
            }

            await _rentalOrderRepository.UpdateAsync(rentalOrder);

            return new BaseResponse { Success = true };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error returning books: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetOverdueRentalsAsync(PaginationDto pagination)
    {
        try
        {
            var overdueRentals = await _rentalOrderRepository.GetOverdueRentalsAsync();
            var paginatedRentals = overdueRentals.Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                                                 .Take(pagination.RecordsPerPage)
                                                 .ToList();

            var response = _mapper.Map<ICollection<RentalOrderResponseDto>>(paginatedRentals);

            return new BaseResponseGeneric<ICollection<RentalOrderResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = overdueRentals.Count()
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<ICollection<RentalOrderResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving overdue rentals: {ex.Message}"
            };
        }
    }

    private string GenerateOrderNumber()
    {
        return $"RO{DateTime.UtcNow:MMddHHmm}{new Random().Next(10, 99)}";
    }

    // Base service implementations
    public async Task<IEnumerable<RentalOrder>> GetAllAsync() => await _rentalOrderRepository.GetAllAsync();
    public async Task<IEnumerable<RentalOrder>> GetAllIncludingDeletedAsync() => await _rentalOrderRepository.GetAllIncludingDeletedAsync();
    public async Task<RentalOrder?> GetByIdAsync(int id) => await _rentalOrderRepository.GetByIdAsync(id);
    public async Task<RentalOrder?> GetByIdIncludingDeletedAsync(int id) => await _rentalOrderRepository.GetByIdIncludingDeletedAsync(id);
    public async Task<IEnumerable<RentalOrder>> FindAsync(System.Linq.Expressions.Expression<Func<RentalOrder, bool>> predicate) => await _rentalOrderRepository.FindAsync(predicate);
    public async Task<RentalOrder> CreateAsync(RentalOrder entity) => await _rentalOrderRepository.CreateAsync(entity);
    public async Task<RentalOrder?> UpdateAsync(RentalOrder entity) => await _rentalOrderRepository.UpdateAsync(entity);
    public async Task<bool> SoftDeleteAsync(int id) => await _rentalOrderRepository.SoftDeleteAsync(id);
    public async Task<bool> RestoreAsync(int id) => await _rentalOrderRepository.RestoreAsync(id);
    public async Task<bool> HardDeleteAsync(int id) => await _rentalOrderRepository.HardDeleteAsync(id);
    public async Task<bool> ExistsAsync(int id) => await _rentalOrderRepository.ExistsAsync(id);
    public async Task<int> CountAsync() => await _rentalOrderRepository.CountAsync();
    public async Task<int> CountIncludingDeletedAsync() => await _rentalOrderRepository.CountIncludingDeletedAsync();
}
