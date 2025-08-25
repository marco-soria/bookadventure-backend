using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class RentalOrderRequestDto
{
    [Required(ErrorMessage = "Customer is required")]
    public int CustomerId { get; set; }
    
    [Required(ErrorMessage = "Due date is required")]
    public DateTime DueDate { get; set; }
    
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
    
    [Required(ErrorMessage = "Must include at least one detail")]
    public List<RentalOrderDetailRequestDto> Details { get; set; } = new();
}

public class RentalOrderDetailRequestDto
{
    [Required(ErrorMessage = "Book is required")]
    public int BookId { get; set; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
    
    [Required(ErrorMessage = "Rental days are required")]
    [Range(1, 365, ErrorMessage = "Rental days must be between 1 and 365")]
    public int RentalDays { get; set; }
    
    [StringLength(200, ErrorMessage = "Notes cannot exceed 200 characters")]
    public string? Notes { get; set; }
}
