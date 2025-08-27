using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class UpdateRentalOrderStatusDto
{
    [Required]
    public int OrderStatus { get; set; }
}
