namespace BookAdventure.Dto.Response;

public class RegisterResponseDto
{
    public string Id { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Message { get; set; } = "User registered successfully";
}
