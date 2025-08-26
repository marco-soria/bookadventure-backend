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
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User ID not found. Please login first or provide userId parameter.");
        }

        var response = await _customerService.GetByUserIdAsync(userId);
        return response.Success ? Ok(response) : NotFound(response);
    }

    // Temporary endpoint for testing without auth
    [HttpGet("profile/test/{userId}")]
    public async Task<IActionResult> GetProfileTest(string userId)
    {
        var response = await _customerService.GetByUserIdAsync(userId);
        return response.Success ? Ok(response) : NotFound(response);
    }
}
