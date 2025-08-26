using BookAdventure.Dto.Request;
using BookAdventure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookAdventure.Api.Controllers;

/// <summary>
/// User Authentication and Registration Controller.
/// Handles user registration (which automatically creates a customer profile), 
/// login, and authentication-related operations.
/// All customers must have user accounts for system access.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICustomerService _customerService;
    private readonly IRentalOrderService _rentalOrderService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ICustomerService customerService, IRentalOrderService rentalOrderService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _customerService = customerService;
        _rentalOrderService = rentalOrderService;
        _logger = logger;
    }

    /// <summary>
    /// Register new user - Public access
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var response = await _userService.RegisterAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// User login - Public access
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _userService.LoginAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get user profile - Authenticated users only
    /// </summary>
    [HttpGet("profile")]
    [Authorize(Policy = "RequireUserRole")]
    public async Task<IActionResult> GetProfile()
    {
        // Get userId from authenticated token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("Invalid token: User ID not found in claims.");
        }

        var response = await _customerService.GetByUserIdAsync(userId);
        return response.Success ? Ok(response) : NotFound(response);
    }

    /// <summary>
    /// Update user profile - Authenticated users can update their own profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize(Policy = "RequireUserRole")]
    public async Task<IActionResult> UpdateProfile([FromBody] CustomerUpdateRequestDto request)
    {
        // Get userId from authenticated token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("Invalid token: User ID not found in claims.");
        }

        // Get customer by userId first to get the customer ID
        var customerResponse = await _customerService.GetByUserIdAsync(userId);
        if (!customerResponse.Success || customerResponse.Data == null)
        {
            return NotFound("Customer profile not found.");
        }

        // Update using the customer ID
        var updateResponse = await _customerService.UpdateAsync(customerResponse.Data.Id, request);
        return updateResponse.Success ? Ok(updateResponse) : BadRequest(updateResponse);
    }

    /// <summary>
    /// Get current user's rental orders - Authenticated users only
    /// </summary>
    [HttpGet("my-rental-orders")]
    [Authorize(Policy = "RequireUserRole")]
    public async Task<IActionResult> GetMyRentalOrders([FromQuery] PaginationDto pagination)
    {
        // Get userId from authenticated token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("Invalid token: User ID not found in claims.");
        }

        // Get customer by userId first to get the customer ID
        var customerResponse = await _customerService.GetByUserIdAsync(userId);
        if (!customerResponse.Success || customerResponse.Data == null)
        {
            return NotFound("Customer profile not found.");
        }

        // Get rental orders for this customer
        var response = await _rentalOrderService.GetByCustomerAsync(customerResponse.Data.Id, pagination);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
