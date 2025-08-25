using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class RentalOrderUpdateRequestDto
{
    public int? CustomerId { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
    
    public List<RentalOrderDetailUpdateRequestDto>? Details { get; set; }
}

public class RentalOrderDetailUpdateRequestDto
{
    public int? BookId { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int? Quantity { get; set; }
    
    [Range(1, 365, ErrorMessage = "Rental days must be between 1 and 365")]
    public int? RentalDays { get; set; }
    
    [StringLength(200, ErrorMessage = "Notes cannot exceed 200 characters")]
    public string? Notes { get; set; }
}
