using BookAdventure.Dto.Request;
using BookAdventure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookAdventure.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    /// <summary>
    /// Get all books with pagination and filters - Public access for browsing
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] BookSearchDto? searchFilters = null)
    {
        // Provide default search filters if none is provided
        searchFilters ??= new BookSearchDto();
        
        var response = await _bookService.GetBooksWithFiltersAsync(searchFilters);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get book by ID - Public access for viewing details
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        var response = await _bookService.GetAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    /// <summary>
    /// Create new book - Admin only
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Post([FromBody] BookRequestDto request)
    {
        var response = await _bookService.AddAsync(request);
        return response.Success ? 
            CreatedAtAction(nameof(Get), new { id = response.Data }, response) : 
            BadRequest(response);
    }

    /// <summary>
    /// Update book - Admin only
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Put(int id, [FromBody] BookUpdateRequestDto request)
    {
        var response = await _bookService.UpdateAsync(id, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Delete book - Admin only
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _bookService.DeleteAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Search books by title - Public access
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest("Title parameter is required");
        }

        var response = await _bookService.SearchAsync(title);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("genre/{genreId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByGenre(int genreId, [FromQuery] PaginationDto? pagination = null)
    {
        // Provide default pagination if none is provided
        pagination ??= new PaginationDto();
        
        var response = await _bookService.GetByGenreAsync(genreId, pagination);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get books by genre name (e.g., "Fantasy", "Science Fiction") - Public access
    /// </summary>
    [HttpGet("genre/name/{genreName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByGenreName(string genreName, [FromQuery] PaginationDto? pagination = null)
    {
        // Provide default pagination if none is provided
        pagination ??= new PaginationDto();
        
        var response = await _bookService.GetByGenreNameAsync(genreName, pagination);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get books by genre name ordered alphabetically - Public access
    /// </summary>
    [HttpGet("genre/name/{genreName}/alphabetical")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByGenreNameAlphabetical(string genreName, [FromQuery] bool descending = false, [FromQuery] PaginationDto? pagination = null)
    {
        pagination ??= new PaginationDto();
        
        // First get the genre to get its ID
        var genreResponse = await _bookService.GetByGenreNameAsync(genreName, new PaginationDto { Page = 1, RecordsPerPage = 1 });
        if (!genreResponse.Success)
        {
            return BadRequest(genreResponse);
        }

        // Use advanced search with genre name filter
        var searchFilters = new BookSearchDto
        {
            Page = pagination.Page,
            RecordsPerPage = pagination.RecordsPerPage,
            Search = "", // We'll filter by genre name in the service
            SortBy = "title",
            SortDescending = descending
        };

        // Create a custom search that filters by genre name
        var response = await _bookService.GetByGenreNameAsync(genreName, pagination);
        
        if (response.Success && response.Data != null)
        {
            // Sort the results alphabetically
            var sortedBooks = descending 
                ? response.Data.OrderByDescending(b => b.Title).ToList()
                : response.Data.OrderBy(b => b.Title).ToList();
            
            response.Data = sortedBooks;
            
            if (response.TotalRecords.HasValue)
            {
                HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
            }
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get books ordered alphabetically by title - Public access
    /// </summary>
    [HttpGet("alphabetical")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAlphabetical([FromQuery] bool descending = false, [FromQuery] PaginationDto? pagination = null)
    {
        pagination ??= new PaginationDto();
        
        var searchFilters = new BookSearchDto
        {
            Page = pagination.Page,
            RecordsPerPage = pagination.RecordsPerPage,
            SortBy = "title",
            SortDescending = descending
        };
        
        var response = await _bookService.GetBooksWithFiltersAsync(searchFilters);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get books filtered by genre and optionally sorted alphabetically - Public access
    /// </summary>
    [HttpGet("genre/{genreId:int}/alphabetical")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByGenreAlphabetical(int genreId, [FromQuery] bool descending = false, [FromQuery] PaginationDto? pagination = null)
    {
        pagination ??= new PaginationDto();
        
        var searchFilters = new BookSearchDto
        {
            Page = pagination.Page,
            RecordsPerPage = pagination.RecordsPerPage,
            GenreId = genreId,
            SortBy = "title",
            SortDescending = descending
        };
        
        var response = await _bookService.GetBooksWithFiltersAsync(searchFilters);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Advanced search with multiple filters - Public access
    /// </summary>
    [HttpGet("advanced-search")]
    [AllowAnonymous]
    public async Task<IActionResult> AdvancedSearch([FromQuery] BookSearchDto searchFilters)
    {
        var response = await _bookService.GetBooksWithFiltersAsync(searchFilters);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get books by author - Public access
    /// </summary>
    [HttpGet("author/{author}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByAuthor(string author, [FromQuery] PaginationDto? pagination = null)
    {
        pagination ??= new PaginationDto();
        
        var searchFilters = new BookSearchDto
        {
            Page = pagination.Page,
            RecordsPerPage = pagination.RecordsPerPage,
            Author = author,
            SortBy = "title"
        };
        
        var response = await _bookService.GetBooksWithFiltersAsync(searchFilters);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get books in stock - Public access
    /// </summary>
    [HttpGet("in-stock")]
    [AllowAnonymous]
    public async Task<IActionResult> GetInStock([FromQuery] PaginationDto? pagination = null)
    {
        pagination ??= new PaginationDto();
        
        var searchFilters = new BookSearchDto
        {
            Page = pagination.Page,
            RecordsPerPage = pagination.RecordsPerPage,
            InStock = true,
            SortBy = "title"
        };
        
        var response = await _bookService.GetBooksWithFiltersAsync(searchFilters);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get all deleted books - Admin only
    /// </summary>
    [HttpGet("deleted")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetDeleted([FromQuery] PaginationDto? pagination = null)
    {
        pagination ??= new PaginationDto();
        
        try
        {
            var response = await _bookService.GetDeletedBooksAsync(pagination);
            
            if (response.Success && response.TotalRecords.HasValue)
            {
                HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
            }
            
            return response.Success ? Ok(response) : BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting deleted books");
            return StatusCode(500, "Internal server error occurred while retrieving deleted books.");
        }
    }

    /// <summary>
    /// Restore a deleted book - Admin only
    /// </summary>
    [HttpPut("{id:int}/restore")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> RestoreBook(int id)
    {
        try
        {
            var response = await _bookService.RestoreBookAsync(id);
            
            if (response.Success)
            {
                _logger.LogInformation("Book with ID {BookId} has been restored", id);
                return Ok(response);
            }
            
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring book with ID {BookId}", id);
            return StatusCode(500, "Internal server error occurred while restoring the book.");
        }
    }
}
