using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookAdventure.Persistence;

public class BookAdventureUserIdentity : IdentityUser
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = default!;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = default!;
    
    [Required]
    [Range(1, 120)]
    public int Age { get; set; }

    [Required]
    [StringLength(20)]
    public string DNI { get; set; } = default!;
    
    // Computed property
    public string FullName => $"{FirstName} {LastName}";
}


