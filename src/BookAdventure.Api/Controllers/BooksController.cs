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
    /// Get all books with pagination - Public access for browsing
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] PaginationDto? pagination = null)
    {
        // Provide default pagination if none is provided
        pagination ??= new PaginationDto();
        
        var response = await _bookService.GetAsync(pagination);
        
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
}
