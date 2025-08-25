using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class GenreUpdateRequestDto
{
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    public string? Name { get; set; }
    
    public bool? Status { get; set; }
}
