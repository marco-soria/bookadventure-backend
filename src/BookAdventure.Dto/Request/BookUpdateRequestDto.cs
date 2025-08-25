using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class BookUpdateRequestDto
{
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; set; }
    
    [StringLength(100, ErrorMessage = "Author cannot exceed 100 characters")]
    public string? Author { get; set; }
    
    [StringLength(13, ErrorMessage = "ISBN cannot exceed 13 characters")]
    public string? ISBN { get; set; }
    
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int? Stock { get; set; }
    
    public int? GenreId { get; set; }
    
    [Url(ErrorMessage = "Must be a valid URL")]
    public string? ImageUrl { get; set; }

    public bool? IsAvailable { get; set; }
}
