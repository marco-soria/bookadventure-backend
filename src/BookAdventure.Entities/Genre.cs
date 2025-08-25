using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Entities;

public class Genre : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;
    
    // Navigation Properties
    public virtual ICollection<Book> Books { get; set; } = new HashSet<Book>();
}
