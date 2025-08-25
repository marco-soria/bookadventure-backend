using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Entities;

public class RentalOrderDetail : BaseEntity
{
    public int Quantity { get; set; } = 1;
    
    public int RentalDays { get; set; }
    
    [MaxLength(200)]
    public string? Notes { get; set; }
    
    // Foreign Keys
    public int RentalOrderId { get; set; }
    public int BookId { get; set; }
    
    // Navigation Properties
    public virtual RentalOrder RentalOrder { get; set; } = default!;
    public virtual Book Book { get; set; } = default!;
}
