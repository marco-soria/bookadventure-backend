using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class RentalOrderUpdateRequestDto
{
    public int? CustomerId { get; set; }
    
    [Range(1, 365, ErrorMessage = "Rental days must be between 1 and 365")]
    public int? RentalDays { get; set; }
    
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
    
    public List<int>? BookIds { get; set; }
}
