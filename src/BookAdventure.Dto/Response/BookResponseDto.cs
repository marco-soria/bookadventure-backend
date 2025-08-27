namespace BookAdventure.Dto.Response;

public class BookResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Related Data
    public int GenreId { get; set; }
    public string GenreName { get; set; } = default!;
}
