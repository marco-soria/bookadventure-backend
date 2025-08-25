using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class GenreRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    public string Name { get; set; } = default!;
    
    public bool Status { get; set; } = true;
}
