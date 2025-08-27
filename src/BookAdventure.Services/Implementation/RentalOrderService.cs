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

    public async Task<RentalOrderCreationResponseDto> CreateRentalOrderAsync(RentalOrderRequestDto request)
    {
        try
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
            {
                return new RentalOrderCreationResponseDto
                {
                    Success = false,
                    ErrorMessage = "Customer not found"
                };
            }

            // Remove duplicate book IDs and validate books availability
            var uniqueBookIds = request.BookIds.Distinct().ToList();
            var processedBooks = new List<BookAvailabilityDto>();
            var unavailableBooks = new List<BookAvailabilityDto>();
            var availableBookIds = new List<int>();

            foreach (var bookId in uniqueBookIds)
            {
                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book == null)
                {
                    unavailableBooks.Add(new BookAvailabilityDto
                    {
                        BookId = bookId,
                        BookTitle = $"Book ID {bookId}",
                        BookAuthor = "Unknown",
                        CurrentStock = 0,
                        Reason = "Book not found"
                    });
                    continue;
                }

                if (book.Stock < 1 || !book.IsAvailable)
                {
                    unavailableBooks.Add(new BookAvailabilityDto
                    {
                        BookId = bookId,
                        BookTitle = book.Title,
                        BookAuthor = book.Author,
                        CurrentStock = book.Stock,
                        Reason = book.Stock < 1 ? "Out of stock" : "Not available"
                    });
                }
                else
                {
                    processedBooks.Add(new BookAvailabilityDto
                    {
                        BookId = bookId,
                        BookTitle = book.Title,
                        BookAuthor = book.Author,
                        CurrentStock = book.Stock,
                        Reason = "Added successfully"
                    });
                    availableBookIds.Add(bookId);
                }
            }

            // Decision logic based on availability and user preference
            if (unavailableBooks.Any() && !request.AllowPartialOrder)
            {
                // Strict mode: fail if any book is unavailable
                return new RentalOrderCreationResponseDto
                {
                    Success = false,
                    ErrorMessage = $"The following books are not available: {string.Join(", ", unavailableBooks.Select(b => $"'{b.BookTitle}' ({b.Reason})"))}. Set 'allowPartialOrder' to true to create order with available books only.",
                    ProcessedBooks = processedBooks,
                    UnavailableBooks = unavailableBooks,
                    IsPartialOrder = false
                };
            }

            if (!availableBookIds.Any())
            {
                // No books available at all
                return new RentalOrderCreationResponseDto
                {
                    Success = false,
                    ErrorMessage = "No books are available for rental",
                    ProcessedBooks = processedBooks,
                    UnavailableBooks = unavailableBooks,
                    IsPartialOrder = false
                };
            }

            // Create rental order with available books
            var dueDate = DateTime.UtcNow.AddDays(request.RentalDays);
            var rentalOrder = new RentalOrder
            {
                CustomerId = request.CustomerId,
                OrderDate = DateTime.UtcNow,
                DueDate = dueDate,
                OrderNumber = GenerateOrderNumber(),
                OrderStatus = OrderStatus.Active,
                Notes = request.Notes,
                RentalOrderDetails = new List<RentalOrderDetail>()
            };

            // Create rental order details - one detail per available book, quantity always 1
            foreach (var bookId in availableBookIds)
            {
                var book = await _bookRepository.GetByIdAsync(bookId);
                var rentalOrderDetail = new RentalOrderDetail
                {
                    BookId = bookId,
                    Quantity = 1, // Always 1 per book in a library system
                    RentalDays = request.RentalDays,
                    DueDate = dueDate,
                    Notes = null, // Individual book notes can be added via separate endpoint if needed
                    IsReturned = false
                };

                rentalOrder.RentalOrderDetails.Add(rentalOrderDetail);

                // Update book stock and availability
                book!.Stock -= 1;
                book.IsAvailable = book.Stock > 0;
                await _bookRepository.UpdateAsync(book);
            }

            var createdRentalOrder = await _rentalOrderRepository.CreateAsync(rentalOrder);

            return new RentalOrderCreationResponseDto
            {
                Success = true,
                RentalOrderId = createdRentalOrder.Id,
                ProcessedBooks = processedBooks,
                UnavailableBooks = unavailableBooks,
                IsPartialOrder = unavailableBooks.Any(),
                ErrorMessage = unavailableBooks.Any() 
                    ? $"Partial order created. {unavailableBooks.Count} book(s) were not available: {string.Join(", ", unavailableBooks.Select(b => b.BookTitle))}"
                    : null
            };
        }
        catch (Exception ex)
        {
            return new RentalOrderCreationResponseDto
            {
                Success = false,
                ErrorMessage = $"Error creating rental order: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> UpdateAsync(int id, RentalOrderUpdateRequestDto request)
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
                // Update only provided fields
                if (request.CustomerId.HasValue)
                {
                    // Validate customer exists
                    var customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value);
                    if (customer == null)
                    {
                        return new BaseResponse
                        {
                            Success = false,
                            ErrorMessage = "Customer not found"
                        };
                    }
                    existingRentalOrder.CustomerId = request.CustomerId.Value;
                }

                if (request.RentalDays.HasValue)
                {
                    var newDueDate = existingRentalOrder.OrderDate.AddDays(request.RentalDays.Value);
                    existingRentalOrder.DueDate = newDueDate;
                    
                    // Update all details with new due date
                    foreach (var detail in existingRentalOrder.RentalOrderDetails)
                    {
                        detail.DueDate = newDueDate;
                        detail.RentalDays = request.RentalDays.Value;
                    }
                }
                
                if (request.Notes != null)
                    existingRentalOrder.Notes = request.Notes;

                // Handle book changes if provided
                if (request.BookIds != null && request.BookIds.Any())
                {
                    var uniqueBookIds = request.BookIds.Distinct().ToList();
                    var currentBookIds = existingRentalOrder.RentalOrderDetails
                        .Where(d => !d.IsReturned)
                        .Select(d => d.BookId)
                        .ToList();

                    // Books to remove (restore stock)
                    var booksToRemove = currentBookIds.Except(uniqueBookIds).ToList();
                    foreach (var bookId in booksToRemove)
                    {
                        var detail = existingRentalOrder.RentalOrderDetails
                            .First(d => d.BookId == bookId && !d.IsReturned);
                        detail.IsReturned = true;
                        detail.ReturnDate = DateTime.UtcNow;

                        var book = await _bookRepository.GetByIdAsync(bookId);
                        if (book != null)
                        {
                            book.Stock += detail.Quantity;
                            book.IsAvailable = book.Stock > 0;
                            await _bookRepository.UpdateAsync(book);
                        }
                    }

                    // Books to add (check stock and add)
                    var booksToAdd = uniqueBookIds.Except(currentBookIds).ToList();
                    foreach (var bookId in booksToAdd)
                    {
                        var book = await _bookRepository.GetByIdAsync(bookId);
                        if (book == null)
                        {
                            return new BaseResponse
                            {
                                Success = false,
                                ErrorMessage = $"Book with ID {bookId} not found"
                            };
                        }

                        if (book.Stock < 1 || !book.IsAvailable)
                        {
                            return new BaseResponse
                            {
                                Success = false,
                                ErrorMessage = $"Book '{book.Title}' is not available"
                            };
                        }

                        // Add new detail
                        var newDetail = new RentalOrderDetail
                        {
                            BookId = bookId,
                            Quantity = 1,
                            RentalDays = request.RentalDays ?? existingRentalOrder.RentalOrderDetails.First().RentalDays,
                            DueDate = request.RentalDays.HasValue 
                                ? existingRentalOrder.OrderDate.AddDays(request.RentalDays.Value)
                                : existingRentalOrder.DueDate,
                            IsReturned = false
                        };

                        existingRentalOrder.RentalOrderDetails.Add(newDetail);

                        // Update book stock
                        book.Stock -= 1;
                        book.IsAvailable = book.Stock > 0;
                        await _bookRepository.UpdateAsync(book);
                    }
                }
                
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
            var rentalOrder = await _rentalOrderRepository.GetWithDetailsIncludingDeletedAsync(id);
            if (rentalOrder == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Rental order not found"
                };
            }

            // Check if already deleted
            if (rentalOrder.Status == EntityStatus.Deleted)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Rental order is already deleted"
                };
            }

            // Only allow deleting active, returned, overdue orders (not cancelled ones as they're already processed)
            if (rentalOrder.OrderStatus == OrderStatus.Active || 
                rentalOrder.OrderStatus == OrderStatus.Returned || 
                rentalOrder.OrderStatus == OrderStatus.Overdue ||
                rentalOrder.OrderStatus == OrderStatus.Pending)
            {
                // If it's an active order, restore book stock for cancelled orders
                if (rentalOrder.OrderStatus == OrderStatus.Active || rentalOrder.OrderStatus == OrderStatus.Pending)
                {
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
                }

                // Soft delete the rental order
                var deleteResult = await _rentalOrderRepository.SoftDeleteAsync(id);
                if (deleteResult)
                {
                    return new BaseResponse { Success = true };
                }
                else
                {
                    return new BaseResponse
                    {
                        Success = false,
                        ErrorMessage = "Failed to delete rental order"
                    };
                }
            }

            return new BaseResponse
            {
                Success = false,
                ErrorMessage = "Cannot delete cancelled rental orders"
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error deleting rental order: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> CancelRentalOrderAsync(int id)
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
                ErrorMessage = "Cannot cancel completed or already cancelled rental orders"
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

    /// <summary>
    /// Obtiene todas las órdenes de alquiler eliminadas lógicamente con paginación
    /// </summary>
    public async Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetDeletedRentalOrdersAsync(PaginationDto pagination)
    {
        try
        {
            var query = _rentalOrderRepository.QueryIncludingDeleted()
                .Where(ro => ro.Status == EntityStatus.Deleted)
                .Include(ro => ro.Customer)
                .Include(ro => ro.RentalOrderDetails)
                    .ThenInclude(rod => rod.Book);

            var totalRecords = await query.CountAsync();

            var rentalOrders = await query
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage)
                .ToListAsync();

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
                ErrorMessage = $"Error retrieving deleted rental orders: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Restaura una orden de alquiler eliminada lógicamente
    /// </summary>
    public async Task<BaseResponse> RestoreRentalOrderAsync(int id)
    {
        try
        {
            var rentalOrder = await _rentalOrderRepository.GetByIdIncludingDeletedAsync(id);
            if (rentalOrder == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Rental order not found"
                };
            }

            if (rentalOrder.Status != EntityStatus.Deleted)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Rental order is not deleted"
                };
            }

            var result = await _rentalOrderRepository.RestoreAsync(id);
            if (result)
            {
                return new BaseResponse
                {
                    Success = true
                };
            }

            return new BaseResponse
            {
                Success = false,
                ErrorMessage = "Failed to restore rental order"
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error restoring rental order: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtiene todas las órdenes de alquiler para el panel de admin (incluyendo eliminadas) con paginación
    /// </summary>
    public async Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetAllRentalOrdersForAdminAsync(PaginationDto pagination)
    {
        try
        {
            var query = _rentalOrderRepository.QueryIncludingDeleted()
                .Include(ro => ro.Customer)
                .Include(ro => ro.RentalOrderDetails)
                .ThenInclude(rod => rod.Book)
                .ThenInclude(b => b.Genre)
                .OrderBy(ro => ro.Id); // Orden consistente

            var totalRecords = await query.CountAsync();

            var rentalOrders = await query
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage)
                .ToListAsync();

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
                ErrorMessage = $"Error retrieving all rental orders for admin: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> UpdateRentalOrderStatusAsync(int id, int orderStatus)
    {
        try
        {
            // Validate order status is within valid range
            if (!Enum.IsDefined(typeof(OrderStatus), orderStatus))
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid order status"
                };
            }

            var rentalOrder = await _rentalOrderRepository.GetByIdAsync(id);
            if (rentalOrder == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Rental order not found"
                };
            }

            // Update the order status
            rentalOrder.OrderStatus = (OrderStatus)orderStatus;

            // Special logic for specific status changes
            switch ((OrderStatus)orderStatus)
            {
                case OrderStatus.Returned:
                    // When marking as returned, set return date and restore book availability
                    rentalOrder.ReturnDate = DateTime.UtcNow;
                    
                    // Get all rental details and mark them as returned
                    var rentalDetails = await _rentalOrderDetailRepository.Query()
                        .Where(rd => rd.RentalOrderId == id)
                        .ToListAsync();

                    foreach (var detail in rentalDetails)
                    {
                        if (!detail.IsReturned)
                        {
                            detail.IsReturned = true;
                            detail.ReturnDate = DateTime.UtcNow;
                            
                            // Restore book availability
                            var book = await _bookRepository.GetByIdAsync(detail.BookId);
                            if (book != null)
                            {
                                book.IsAvailable = true;
                                await _bookRepository.UpdateAsync(book);
                            }
                            
                            await _rentalOrderDetailRepository.UpdateAsync(detail);
                        }
                    }
                    break;

                case OrderStatus.Cancelled:
                    // When cancelling, restore book availability for non-returned books
                    var cancelDetails = await _rentalOrderDetailRepository.Query()
                        .Where(rd => rd.RentalOrderId == id && !rd.IsReturned)
                        .ToListAsync();

                    foreach (var detail in cancelDetails)
                    {
                        var book = await _bookRepository.GetByIdAsync(detail.BookId);
                        if (book != null)
                        {
                            book.IsAvailable = true;
                            await _bookRepository.UpdateAsync(book);
                        }
                    }
                    break;
            }

            await _rentalOrderRepository.UpdateAsync(rentalOrder);

            return new BaseResponse
            {
                Success = true,
                ErrorMessage = "Rental order status updated successfully"
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error updating rental order status: {ex.Message}"
            };
        }
    }
}
