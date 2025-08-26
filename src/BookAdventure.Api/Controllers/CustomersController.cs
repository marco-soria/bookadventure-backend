using BookAdventure.Dto.Request;
using BookAdventure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookAdventure.Api.Controllers;

/// <summary>
/// Customer Management Controller - Handles customer data operations.
/// Note: Customers are created automatically when users register via UsersController.
/// This controller is used for reading, updating, and managing existing customer data.
/// All customers must have user accounts for authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PaginationDto pagination)
    {
        var response = await _customerService.GetAsync(pagination);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var response = await _customerService.GetAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] CustomerUpdateRequestDto request)
    {
        var response = await _customerService.UpdateAsync(id, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _customerService.DeleteAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("dni/{dni}")]
    public async Task<IActionResult> GetByDni(string dni)
    {
        var response = await _customerService.GetByDniAsync(dni);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Name parameter is required");
        }

        var response = await _customerService.SearchByNameAsync(name);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("dni/{dni}/rented-books")]
    public async Task<IActionResult> GetRentedBooksByDni(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
        {
            return BadRequest("DNI parameter is required");
        }

        var response = await _customerService.GetRentedBooksByDniAsync(dni);
        return response.Success ? Ok(response) : NotFound(response);
    }
}
