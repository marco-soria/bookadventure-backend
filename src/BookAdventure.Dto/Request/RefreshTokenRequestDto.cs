using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = default!;
}
