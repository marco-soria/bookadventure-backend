using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Entities;

public class RentalOrder : BaseEntity
{
    [Required]
    [MaxLength(20)]
    public string OrderNumber { get; set; } = default!;
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? ReturnDate { get; set; }
    
    public DateTime DueDate { get; set; }
    
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    // Foreign Key
    public int CustomerId { get; set; }
    
    // Navigation Properties
    public virtual Customer Customer { get; set; } = default!;
    public virtual ICollection<RentalOrderDetail> RentalOrderDetails { get; set; } = new HashSet<RentalOrderDetail>();
}
