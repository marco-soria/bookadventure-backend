using BookAdventure.Dto.Request;
using BookAdventure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookAdventure.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly ILogger<GenresController> _logger;

    public GenresController(IGenreService genreService, ILogger<GenresController> logger)
    {
        _genreService = genreService;
        _logger = logger;
    }

    /// <summary>
    /// Get all genres - Public access
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        var response = await _genreService.GetAsync();
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get genre by ID - Public access
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        var response = await _genreService.GetAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    /// <summary>
    /// Create new genre - Admin only
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Post([FromBody] GenreRequestDto request)
    {
        var response = await _genreService.AddAsync(request);
        return response.Success ? 
            CreatedAtAction(nameof(Get), new { id = response.Data }, response) : 
            BadRequest(response);
    }

    /// <summary>
    /// Update genre - Admin only
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Put(int id, [FromBody] GenreUpdateRequestDto request)
    {
        var response = await _genreService.UpdateAsync(id, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Delete genre - Admin only
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _genreService.DeleteAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get all genres for admin panel (including deleted) - Admin only
    /// </summary>
    [HttpGet("admin/all")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetAllForAdmin([FromQuery] PaginationDto? pagination = null)
    {
        pagination ??= new PaginationDto();
        
        try
        {
            var response = await _genreService.GetAllGenresForAdminAsync(pagination);
            
            if (response.Success && response.TotalRecords.HasValue)
            {
                HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
            }
            
            return response.Success ? Ok(response) : BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all genres for admin");
            return StatusCode(500, "Internal server error occurred while retrieving all genres.");
        }
    }

    /// <summary>
    /// Get all deleted genres - Admin only
    /// </summary>
    [HttpGet("deleted")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetDeleted([FromQuery] PaginationDto? pagination = null)
    {
        pagination ??= new PaginationDto();
        
        try
        {
            var response = await _genreService.GetDeletedGenresAsync(pagination);
            
            if (response.Success && response.TotalRecords.HasValue)
            {
                HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
            }
            
            return response.Success ? Ok(response) : BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting deleted genres");
            return StatusCode(500, "Internal server error occurred while retrieving deleted genres.");
        }
    }

    /// <summary>
    /// Restore a deleted genre - Admin only
    /// </summary>
    [HttpPut("{id:int}/restore")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> RestoreGenre(int id)
    {
        try
        {
            var response = await _genreService.RestoreGenreAsync(id);
            
            if (response.Success)
            {
                _logger.LogInformation("Genre with ID {GenreId} has been restored", id);
                return Ok(response);
            }
            
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring genre with ID {GenreId}", id);
            return StatusCode(500, "Internal server error occurred while restoring the genre.");
        }
    }
}
