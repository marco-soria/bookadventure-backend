namespace BookAdventure.Dto.Response;

public class CustomerResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string DNI { get; set; } = default!;
    public int Age { get; set; }
    public string? PhoneNumber { get; set; }
    public string? UserId { get; set; } // Link to Identity User
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Statistics
    public int TotalRentalOrders { get; set; }
    public int ActiveRentals { get; set; }
    public int OverdueRentals { get; set; }
}
