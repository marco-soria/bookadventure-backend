using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class BookRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = default!;
    
    [Required(ErrorMessage = "Author is required")]
    [StringLength(100, ErrorMessage = "Author cannot exceed 100 characters")]
    public string Author { get; set; } = default!;
    
    [StringLength(13, ErrorMessage = "ISBN cannot exceed 13 characters")]
    public string? ISBN { get; set; }
    
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }
    
    [Required(ErrorMessage = "Genre is required")]
    public int GenreId { get; set; }
    
    [Url(ErrorMessage = "Must be a valid URL")]
    public string? ImageUrl { get; set; }
}
