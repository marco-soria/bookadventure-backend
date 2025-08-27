using BookAdventure.Dto.Request;
using BookAdventure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookAdventure.Api.Controllers;

/// <summary>
/// Admin Controller - Provides administrative operations for managing deleted entities and system overview
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdminRole")]
public class AdminController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ICustomerService _customerService;
    private readonly IGenreService _genreService;
    private readonly IRentalOrderService _rentalOrderService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IBookService bookService,
        ICustomerService customerService,
        IGenreService genreService,
        IRentalOrderService rentalOrderService,
        ILogger<AdminController> logger)
    {
        _bookService = bookService;
        _customerService = customerService;
        _genreService = genreService;
        _rentalOrderService = rentalOrderService;
        _logger = logger;
    }

    /// <summary>
    /// Get deleted entities summary - Admin only
    /// Returns count of deleted entities by type
    /// </summary>
    [HttpGet("deleted-summary")]
    public async Task<IActionResult> GetDeletedSummary()
    {
        try
        {
            var booksTask = _bookService.CountIncludingDeletedAsync();
            var customersTask = _customerService.CountIncludingDeletedAsync();
            var genresTask = _genreService.CountIncludingDeletedAsync();
            var rentalOrdersTask = _rentalOrderService.CountIncludingDeletedAsync();

            await Task.WhenAll(booksTask, customersTask, genresTask, rentalOrdersTask);

            var booksTotal = await booksTask;
            var customersTotal = await customersTask;
            var genresTotal = await genresTask;
            var rentalOrdersTotal = await rentalOrdersTask;

            // Conteo de activos para calcular eliminados
            var booksActive = await _bookService.CountAsync();
            var customersActive = await _customerService.CountAsync();
            var genresActive = await _genreService.CountAsync();
            var rentalOrdersActive = await _rentalOrderService.CountAsync();

            var summary = new
            {
                DeletedEntities = new
                {
                    Books = booksTotal - booksActive,
                    Customers = customersTotal - customersActive,
                    Genres = genresTotal - genresActive,
                    RentalOrders = rentalOrdersTotal - rentalOrdersActive
                },
                TotalEntities = new
                {
                    Books = booksTotal,
                    Customers = customersTotal,
                    Genres = genresTotal,
                    RentalOrders = rentalOrdersTotal
                },
                ActiveEntities = new
                {
                    Books = booksActive,
                    Customers = customersActive,
                    Genres = genresActive,
                    RentalOrders = rentalOrdersActive
                }
            };

            return Ok(new
            {
                Success = true,
                Data = summary,
                Message = "Deleted entities summary retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting deleted entities summary");
            return StatusCode(500, "Internal server error occurred while retrieving deleted entities summary.");
        }
    }

    /// <summary>
    /// Bulk restore multiple entities - Admin only
    /// </summary>
    [HttpPost("bulk-restore")]
    public async Task<IActionResult> BulkRestore([FromBody] BulkRestoreRequestDto request)
    {
        if (request == null || 
            (request.BookIds?.Count == 0 && 
             request.CustomerIds?.Count == 0 && 
             request.GenreIds?.Count == 0 && 
             request.RentalOrderIds?.Count == 0))
        {
            return BadRequest("At least one entity ID must be provided for restoration");
        }

        try
        {
            var results = new
            {
                Books = new List<object>(),
                Customers = new List<object>(),
                Genres = new List<object>(),
                RentalOrders = new List<object>()
            };

            // Restore books
            if (request.BookIds?.Count > 0)
            {
                foreach (var bookId in request.BookIds)
                {
                    var result = await _bookService.RestoreBookAsync(bookId);
                    results.Books.Add(new { Id = bookId, Success = result.Success, Error = result.ErrorMessage });
                }
            }

            // Restore customers
            if (request.CustomerIds?.Count > 0)
            {
                foreach (var customerId in request.CustomerIds)
                {
                    var result = await _customerService.RestoreCustomerAsync(customerId);
                    results.Customers.Add(new { Id = customerId, Success = result.Success, Error = result.ErrorMessage });
                }
            }

            // Restore genres
            if (request.GenreIds?.Count > 0)
            {
                foreach (var genreId in request.GenreIds)
                {
                    var result = await _genreService.RestoreGenreAsync(genreId);
                    results.Genres.Add(new { Id = genreId, Success = result.Success, Error = result.ErrorMessage });
                }
            }

            // Restore rental orders
            if (request.RentalOrderIds?.Count > 0)
            {
                foreach (var rentalOrderId in request.RentalOrderIds)
                {
                    var result = await _rentalOrderService.RestoreRentalOrderAsync(rentalOrderId);
                    results.RentalOrders.Add(new { Id = rentalOrderId, Success = result.Success, Error = result.ErrorMessage });
                }
            }

            return Ok(new
            {
                Success = true,
                Data = results,
                Message = "Bulk restore operation completed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk restore operation");
            return StatusCode(500, "Internal server error occurred during bulk restore operation.");
        }
    }

    /// <summary>
    /// Get detailed deleted entities for review before restoration - Admin only
    /// </summary>
    [HttpGet("deleted-entities")]
    public async Task<IActionResult> GetDeletedEntities([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var pagination = new PaginationDto { Page = page, RecordsPerPage = pageSize };
            
            var booksTask = _bookService.GetDeletedBooksAsync(pagination);
            var customersTask = _customerService.GetDeletedCustomersAsync(pagination);
            var genresTask = _genreService.GetDeletedGenresAsync(pagination);
            var rentalOrdersTask = _rentalOrderService.GetDeletedRentalOrdersAsync(pagination);

            await Task.WhenAll(booksTask, customersTask, genresTask, rentalOrdersTask);

            var booksResult = await booksTask;
            var customersResult = await customersTask;
            var genresResult = await genresTask;
            var rentalOrdersResult = await rentalOrdersTask;

            var response = new
            {
                Books = new 
                {
                    Data = booksResult.Success ? booksResult.Data : null,
                    TotalRecords = booksResult.TotalRecords,
                    Error = booksResult.Success ? null : booksResult.ErrorMessage
                },
                Customers = new 
                {
                    Data = customersResult.Success ? customersResult.Data : null,
                    TotalRecords = customersResult.TotalRecords,
                    Error = customersResult.Success ? null : customersResult.ErrorMessage
                },
                Genres = new 
                {
                    Data = genresResult.Success ? genresResult.Data : null,
                    TotalRecords = genresResult.TotalRecords,
                    Error = genresResult.Success ? null : genresResult.ErrorMessage
                },
                RentalOrders = new 
                {
                    Data = rentalOrdersResult.Success ? rentalOrdersResult.Data : null,
                    TotalRecords = rentalOrdersResult.TotalRecords,
                    Error = rentalOrdersResult.Success ? null : rentalOrdersResult.ErrorMessage
                },
                Pagination = new 
                {
                    Page = page,
                    PageSize = pageSize
                }
            };

            return Ok(new
            {
                Success = true,
                Data = response,
                Message = "Deleted entities retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting detailed deleted entities");
            return StatusCode(500, "Internal server error occurred while retrieving detailed deleted entities.");
        }
    }
}
