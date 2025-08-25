using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Entities;

public class Book : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = default!;
    
    [Required]
    [MaxLength(100)]
    public string Author { get; set; } = default!;
    
    [MaxLength(13)]
    public string? ISBN { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public int Stock { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    // Foreign Key
    public int GenreId { get; set; }
    
    // Navigation Properties
    public virtual Genre Genre { get; set; } = default!;
    public virtual ICollection<RentalOrderDetail> RentalOrderDetails { get; set; } = new HashSet<RentalOrderDetail>();
}
