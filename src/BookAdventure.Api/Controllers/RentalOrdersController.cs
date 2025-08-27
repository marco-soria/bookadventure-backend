using BookAdventure.Dto.Request;
using BookAdventure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookAdventure.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalOrdersController : ControllerBase
{
    private readonly IRentalOrderService _rentalOrderService;
    private readonly ICustomerService _customerService;
    private readonly ILogger<RentalOrdersController> _logger;

    public RentalOrdersController(IRentalOrderService rentalOrderService, ICustomerService customerService, ILogger<RentalOrdersController> logger)
    {
        _rentalOrderService = rentalOrderService;
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Helper method to validate user ownership of customer data
    /// </summary>
    private async Task<(bool IsValid, int CustomerId, string ErrorMessage)> ValidateUserOwnershipAsync(int? requestedCustomerId = null)
    {
        // Get userId from authenticated token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return (false, 0, "Invalid token: User ID not found in claims.");
        }

        // Check if user is admin (admins can access any customer data)
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        bool isAdmin = userRoles.Contains("Admin");

        // Get customer by userId to get the actual customer ID for this user
        var customerResponse = await _customerService.GetByUserIdAsync(userId);
        if (!customerResponse.Success || customerResponse.Data == null)
        {
            return (false, 0, "Customer profile not found for current user.");
        }

        int userCustomerId = customerResponse.Data.Id;

        // If a specific customer ID was requested, validate ownership
        if (requestedCustomerId.HasValue)
        {
            if (!isAdmin && userCustomerId != requestedCustomerId.Value)
            {
                return (false, 0, "Access denied: You can only access your own data.");
            }
        }

        return (true, userCustomerId, string.Empty);
    }

    /// <summary>
    /// Helper method to validate user ownership of a specific rental order
    /// </summary>
    private async Task<(bool IsValid, string ErrorMessage)> ValidateRentalOrderOwnershipAsync(int rentalOrderId)
    {
        // Get userId from authenticated token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return (false, "Invalid token: User ID not found in claims.");
        }

        // Check if user is admin (admins can access any rental order)
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        bool isAdmin = userRoles.Contains("Admin");

        if (isAdmin)
        {
            return (true, string.Empty); // Admins can access any rental order
        }

        // Get customer by userId to get the actual customer ID for this user
        var customerResponse = await _customerService.GetByUserIdAsync(userId);
        if (!customerResponse.Success || customerResponse.Data == null)
        {
            return (false, "Customer profile not found for current user.");
        }

        // Get the rental order to check its customer ID
        var rentalOrderResponse = await _rentalOrderService.GetAsync(rentalOrderId);
        if (!rentalOrderResponse.Success || rentalOrderResponse.Data == null)
        {
            return (false, "Rental order not found.");
        }

        // Check if the rental order belongs to this customer
        if (rentalOrderResponse.Data.CustomerId != customerResponse.Data.Id)
        {
            return (false, "Access denied: You can only access your own rental orders.");
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// Get all rental orders - Admin only
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Get([FromQuery] PaginationDto pagination)
    {
        var response = await _rentalOrderService.GetAsync(pagination);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get rental order by ID - Admin only
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Get(int id)
    {
        var response = await _rentalOrderService.GetAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    /// <summary>
    /// Create new rental order - Authenticated users and admins
    /// Users can only create orders for their own customer account
    /// Admins can create orders for any customer
    /// </summary>
    [HttpPost]
    [Authorize] // Allow both users and admins
    public async Task<IActionResult> Post([FromBody] RentalOrderRequestDto request)
    {
        // Check if user is admin
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        bool isAdmin = userRoles.Contains("Admin");

        if (!isAdmin)
        {
            // For regular users, validate ownership of the customer ID
            var validation = await ValidateUserOwnershipAsync(request.CustomerId);
            if (!validation.IsValid)
            {
                return Unauthorized(validation.ErrorMessage);
            }
        }
        else
        {
            // For admins, just validate that the customer exists
            var customerResponse = await _customerService.GetAsync(request.CustomerId);
            if (!customerResponse.Success || customerResponse.Data == null)
            {
                return BadRequest("Customer not found");
            }
        }

        var response = await _rentalOrderService.CreateRentalOrderAsync(request);
        
        if (response.Success)
        {
            if (response.IsPartialOrder)
            {
                // 206 Partial Content - some books were processed, some weren't
                return StatusCode(206, response);
            }
            else
            {
                // 201 Created - all books were processed successfully
                return CreatedAtAction(nameof(Get), new { id = response.RentalOrderId }, response);
            }
        }
        else
        {
            // 400 Bad Request - order failed completely
            return BadRequest(response);
        }
    }

    /// <summary>
    /// Rent single book - Authenticated users and admins
    /// Users can only create orders for their own customer account
    /// Admins can create orders for any customer
    /// </summary>
    [HttpPost("rent-single-book")]
    [Authorize] // Allow both users and admins
    public async Task<IActionResult> RentSingleBook([FromBody] SingleBookRentalRequestDto request)
    {
        // Check if user is admin
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        bool isAdmin = userRoles.Contains("Admin");

        if (!isAdmin)
        {
            // For regular users, validate ownership of the customer ID
            var validation = await ValidateUserOwnershipAsync(request.CustomerId);
            if (!validation.IsValid)
            {
                return Unauthorized(validation.ErrorMessage);
            }
        }
        else
        {
            // For admins, just validate that the customer exists
            var customerResponse = await _customerService.GetAsync(request.CustomerId);
            if (!customerResponse.Success || customerResponse.Data == null)
            {
                return BadRequest("Customer not found");
            }
        }

        // Convert single book request to full rental order request
        var rentalOrderRequest = new RentalOrderRequestDto
        {
            CustomerId = request.CustomerId,
            RentalDays = request.RentalDays,
            Notes = request.OrderNotes,
            BookIds = new List<int> { request.BookId }
        };

        var response = await _rentalOrderService.CreateRentalOrderAsync(rentalOrderRequest);
        
        if (response.Success)
        {
            return CreatedAtAction(nameof(Get), new { id = response.RentalOrderId }, response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    /// <summary>
    /// Update rental order - Admin only
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Put(int id, [FromBody] RentalOrderUpdateRequestDto request)
    {
        var response = await _rentalOrderService.UpdateAsync(id, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Delete rental order (Soft Delete) - Admin only
    /// This performs logical deletion for audit purposes
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _rentalOrderService.DeleteAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Cancel rental order (Business Logic) - Admin only
    /// This changes the order status to Cancelled and restores book stock
    /// </summary>
    [HttpPut("{id:int}/cancel")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> CancelRentalOrder(int id)
    {
        var response = await _rentalOrderService.CancelRentalOrderAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get rental orders by customer - DEPRECATED: Use GET /my-orders instead
    /// This endpoint is maintained for admin use only
    /// Users should use GET /my-orders for their own rental orders
    /// </summary>
    [HttpGet("customer/{customerId:int}")]
    [Authorize(Policy = "RequireAdminRole")] // Changed to admin only
    public async Task<IActionResult> GetByCustomer(int customerId, [FromQuery] PaginationDto pagination)
    {
        // This endpoint is now admin-only for security reasons
        // Regular users should use GET /my-orders instead
        var response = await _rentalOrderService.GetByCustomerAsync(customerId, pagination);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Return books from rental order - Authenticated users (for their orders) and admins
    /// </summary>
    [HttpPost("{id:int}/return")]
    [Authorize] // Allow both users and admins
    public async Task<IActionResult> ReturnBooks(int id, [FromBody] List<int> bookIds)
    {
        // Check if user is admin
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        bool isAdmin = userRoles.Contains("Admin");

        if (!isAdmin)
        {
            // For regular users, validate ownership of this rental order
            var validation = await ValidateRentalOrderOwnershipAsync(id);
            if (!validation.IsValid)
            {
                return Unauthorized(validation.ErrorMessage);
            }
        }
        // Admins can return books from any rental order

        var response = await _rentalOrderService.ReturnBooksAsync(id, bookIds);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get current user's rental orders - Authenticated users and admins
    /// For users: returns their own rental orders
    /// For admins: can optionally pass customerId parameter to get specific customer's orders
    /// </summary>
    [HttpGet("my-orders")]
    [Authorize] // Allow both users and admins
    public async Task<IActionResult> GetMyRentalOrders([FromQuery] PaginationDto pagination)
    {
        // Validate user and get customer ID
        var validation = await ValidateUserOwnershipAsync();
        if (!validation.IsValid)
        {
            return Unauthorized(validation.ErrorMessage);
        }

        var response = await _rentalOrderService.GetByCustomerAsync(validation.CustomerId, pagination);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Get overdue rentals - Admin only
    /// </summary>
    [HttpGet("overdue")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetOverdue([FromQuery] PaginationDto pagination)
    {
        var response = await _rentalOrderService.GetOverdueRentalsAsync(pagination);
        
        if (response.Success && response.TotalRecords.HasValue)
        {
            HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
        }
        
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Create new rental order for current user - Simplified endpoint for regular users only
    /// Automatically uses the authenticated user's customer account
    /// </summary>
    [HttpPost("create-for-me")]
    [Authorize(Policy = "RequireUserRole")] // Keep this as user-only since admins use the main endpoint
    public async Task<IActionResult> CreateRentalOrderForMe([FromBody] CreateRentalOrderForUserDto request)
    {
        // Get current user's customer ID
        var validation = await ValidateUserOwnershipAsync();
        if (!validation.IsValid)
        {
            return Unauthorized(validation.ErrorMessage);
        }

        // Create the rental order request with the user's customer ID
        var rentalOrderRequest = new RentalOrderRequestDto
        {
            CustomerId = validation.CustomerId,
            RentalDays = request.RentalDays,
            Notes = request.Notes,
            BookIds = request.BookIds,
            AllowPartialOrder = request.AllowPartialOrder
        };

        var response = await _rentalOrderService.CreateRentalOrderAsync(rentalOrderRequest);
        
        if (response.Success)
        {
            if (response.IsPartialOrder)
            {
                return StatusCode(206, response);
            }
            else
            {
                return CreatedAtAction(nameof(Get), new { id = response.RentalOrderId }, response);
            }
        }
        else
        {
            return BadRequest(response);
        }
    }

    /// <summary>
    /// Get all rental orders for admin panel (including deleted) - Admin only
    /// </summary>
    [HttpGet("admin/all")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetAllForAdmin([FromQuery] PaginationDto? pagination = null)
    {
        pagination ??= new PaginationDto();
        
        try
        {
            var response = await _rentalOrderService.GetAllRentalOrdersForAdminAsync(pagination);
            
            if (response.Success && response.TotalRecords.HasValue)
            {
                HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
            }
            
            return response.Success ? Ok(response) : BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all rental orders for admin");
            return StatusCode(500, "Internal server error occurred while retrieving all rental orders.");
        }
    }

    /// <summary>
    /// Get all deleted rental orders - Admin only
    /// </summary>
    [HttpGet("deleted")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetDeleted([FromQuery] PaginationDto? pagination = null)
    {
        pagination ??= new PaginationDto();
        
        try
        {
            var response = await _rentalOrderService.GetDeletedRentalOrdersAsync(pagination);
            
            if (response.Success && response.TotalRecords.HasValue)
            {
                HttpContext.Response.Headers.Append("TotalRecordsQuantity", response.TotalRecords.Value.ToString());
            }
            
            return response.Success ? Ok(response) : BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving deleted rental orders");
            return StatusCode(500, "Internal server error occurred while retrieving deleted rental orders.");
        }
    }

    /// <summary>
    /// Restore a deleted rental order - Admin only
    /// </summary>
    [HttpPut("{id:int}/restore")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> RestoreRentalOrder(int id)
    {
        try
        {
            var response = await _rentalOrderService.RestoreRentalOrderAsync(id);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring rental order with ID {Id}", id);
            return StatusCode(500, "Internal server error occurred while restoring the rental order.");
        }
    }

    /// <summary>
    /// Update rental order status - Admin only
    /// </summary>
    [HttpPut("{id:int}/status")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> UpdateRentalOrderStatus(int id, [FromBody] UpdateRentalOrderStatusDto request)
    {
        try
        {
            var response = await _rentalOrderService.UpdateRentalOrderStatusAsync(id, request.OrderStatus);
            
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating rental order status with ID {Id}", id);
            return StatusCode(500, "Internal server error occurred while updating the rental order status.");
        }
    }
}
