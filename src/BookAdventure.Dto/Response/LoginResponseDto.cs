namespace BookAdventure.Dto.Response;

public class LoginResponseDto
{
    public string Id { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTime ExpirationDate { get; set; }
    public string RefreshToken { get; set; } = default!;
    public DateTime RefreshTokenExpirationDate { get; set; }
    public List<string> Roles { get; set; } = new();
}
