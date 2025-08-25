using System;

namespace BookAdventure.Entities.Info;

public class BookInfo
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    
    public int GenreId { get; set; }
    public string Genre { get; set; } = default!;
   
    public string? ImageUrl { get; set; }
    
}
