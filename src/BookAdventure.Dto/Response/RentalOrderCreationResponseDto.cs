namespace BookAdventure.Dto.Response;

public class RentalOrderCreationResponseDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? RentalOrderId { get; set; }
    
    /// <summary>
    /// Books that were successfully added to the rental order
    /// </summary>
    public List<BookAvailabilityDto> ProcessedBooks { get; set; } = new();
    
    /// <summary>
    /// Books that were not available and excluded from the order
    /// </summary>
    public List<BookAvailabilityDto> UnavailableBooks { get; set; } = new();
    
    /// <summary>
    /// Indicates if this was a partial order (some books were excluded)
    /// </summary>
    public bool IsPartialOrder { get; set; }
}

public class BookAvailabilityDto
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = default!;
    public string BookAuthor { get; set; } = default!;
    public int CurrentStock { get; set; }
    public string Reason { get; set; } = default!; // "Added successfully", "Out of stock", "Not found", etc.
}
