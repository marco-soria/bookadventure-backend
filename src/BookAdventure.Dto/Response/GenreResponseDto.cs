namespace BookAdventure.Dto.Response;

public class GenreResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Statistics
    public int TotalBooks { get; set; }
}
