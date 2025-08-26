using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class SingleBookRentalRequestDto
{
    [Required(ErrorMessage = "Customer is required")]
    public int CustomerId { get; set; }
    
    [Required(ErrorMessage = "Book is required")]
    public int BookId { get; set; }
    
    [Required(ErrorMessage = "Rental days are required")]
    [Range(1, 365, ErrorMessage = "Rental days must be between 1 and 365")]
    public int RentalDays { get; set; }
    
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
    
    [StringLength(200, ErrorMessage = "Book notes cannot exceed 200 characters")]
    public string? BookNotes { get; set; }
}
