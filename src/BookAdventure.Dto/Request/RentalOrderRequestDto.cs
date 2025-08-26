using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class RentalOrderRequestDto
{
    [Required(ErrorMessage = "Customer is required")]
    public int CustomerId { get; set; }
    
    [Required(ErrorMessage = "Rental days are required")]
    [Range(1, 365, ErrorMessage = "Rental days must be between 1 and 365")]
    public int RentalDays { get; set; }
    
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
    
    [Required(ErrorMessage = "Must include at least one book")]
    public List<int> BookIds { get; set; } = new();
    
    /// <summary>
    /// If true, creates partial order with available books only.
    /// If false, fails entire order if any book is unavailable.
    /// Default: false (fail all if any unavailable)
    /// </summary>
    public bool AllowPartialOrder { get; set; } = false;
}
