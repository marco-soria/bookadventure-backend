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
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ICustomerService customerService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _customerService = customerService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var response = await _userService.RegisterAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _userService.LoginAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile([FromQuery] string? userId = null)
    {
        // Try to get userId from query parameter first, then from token
        var targetUserId = userId ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(targetUserId))
        {
            return BadRequest("User ID is required. Provide userId parameter or login first.");
        }

        var response = await _customerService.GetByUserIdAsync(targetUserId);
        return response.Success ? Ok(response) : NotFound(response);
    }
}
