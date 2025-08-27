namespace BookAdventure.Dto.Request;

/// <summary>
/// DTO for bulk restore operations - Admin only
/// </summary>
public class BulkRestoreRequestDto
{
    public List<int>? BookIds { get; set; }
    public List<int>? CustomerIds { get; set; }
    public List<int>? GenreIds { get; set; }
    public List<int>? RentalOrderIds { get; set; }
}
