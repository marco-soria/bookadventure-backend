namespace BookAdventure.Dto.Response;

public class RentedBookResponseDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string? ISBN { get; set; }
    public string Genre { get; set; } = default!;
    public string? ImageUrl { get; set; }
    
    // Rental Information
    public int RentalOrderId { get; set; }
    public string OrderNumber { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsReturned { get; set; }
    public int Quantity { get; set; }
    public int RentalDays { get; set; }
    public string? Notes { get; set; }
    public string OrderStatus { get; set; } = default!;
    
    // Computed Properties
    public bool IsOverdue => !IsReturned && DateTime.Now > DueDate;
    public int DaysOverdue => IsOverdue ? (DateTime.Now - DueDate).Days : 0;
}
