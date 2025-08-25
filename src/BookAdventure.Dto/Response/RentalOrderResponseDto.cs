namespace BookAdventure.Dto.Response;

public class RentalOrderResponseDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public DateTime DueDate { get; set; }
    public string OrderStatus { get; set; } = default!;
    public string? Notes { get; set; }
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Customer Information
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public string CustomerDNI { get; set; } = default!;
    
    // Order Details
    public List<RentalOrderDetailResponseDto> Details { get; set; } = new();
    
    // Summary Information
    public int TotalBooks { get; set; }
    public int ActiveBooks { get; set; }
    public int ReturnedBooks { get; set; }
    public bool HasOverdueBooks { get; set; }
}

public class RentalOrderDetailResponseDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int RentalDays { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsReturned { get; set; }
    public bool IsOverdue { get; set; }
    public string? Notes { get; set; }
    
    // Book Information
    public int BookId { get; set; }
    public string BookTitle { get; set; } = default!;
    public string BookAuthor { get; set; } = default!;
    public string? BookISBN { get; set; }
    public string? BookImageUrl { get; set; }
    public string? BookGenre { get; set; }
    
    // Rental Order Information
    public int RentalOrderId { get; set; }
    public string RentalOrderNumber { get; set; } = default!;
}
