using BookAdventure.Dto.Request;
using BookAdventure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookAdventure.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalOrdersController : ControllerBase
{
    private readonly IRentalOrderService _rentalOrderService;
    private readonly ILogger<RentalOrdersController> _logger;

    public RentalOrdersController(IRentalOrderService rentalOrderService, ILogger<RentalOrdersController> logger)
    {
        _rentalOrderService = rentalOrderService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PaginationDto pagination)
    {
        var response = await _rentalOrderService.GetAsync(pagination);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var response = await _rentalOrderService.GetAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RentalOrderRequestDto request)
    {
        var response = await _rentalOrderService.CreateRentalOrderAsync(request);
        return response.Success ? 
            CreatedAtAction(nameof(Get), new { id = response.Data }, response) : 
            BadRequest(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] RentalOrderRequestDto request)
    {
        var response = await _rentalOrderService.UpdateAsync(id, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _rentalOrderService.DeleteAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomer(int customerId, [FromQuery] PaginationDto pagination)
    {
        var response = await _rentalOrderService.GetByCustomerAsync(customerId, pagination);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("{id:int}/return")]
    public async Task<IActionResult> ReturnBooks(int id, [FromBody] List<int> bookIds)
    {
        var response = await _rentalOrderService.ReturnBooksAsync(id, bookIds);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue([FromQuery] PaginationDto pagination)
    {
        var response = await _rentalOrderService.GetOverdueRentalsAsync(pagination);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
