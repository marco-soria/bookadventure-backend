using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

/// <summary>
/// DTO for creating rental orders for the current authenticated user
/// Customer ID is automatically obtained from the JWT token
/// </summary>
public class CreateRentalOrderForUserDto
{
    [Required]
    [Range(1, 365, ErrorMessage = "Rental days must be between 1 and 365")]
    public int RentalDays { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one book must be selected")]
    public List<int> BookIds { get; set; } = new();

    public bool AllowPartialOrder { get; set; } = false;
}
