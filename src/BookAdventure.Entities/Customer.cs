using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Entities;

public class Customer : BaseEntity
{
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = default!;
    
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = default!;
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = default!;
    
    [Required]
    [MaxLength(20)]
    public string DNI { get; set; } = default!;

    [Required]
    [Range(1, 120)]
    public int Age { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    // Computed property for backwards compatibility
    public string FullName => $"{FirstName} {LastName}";
    
    // Navigation Properties
    public virtual ICollection<RentalOrder> RentalOrders { get; set; } = new HashSet<RentalOrder>();
}