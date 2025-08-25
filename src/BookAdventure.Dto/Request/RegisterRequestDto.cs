using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class RegisterRequestDto
{
    [Required]
    [StringLength(200)]
    public string FirstName { get; set; } = default!;

    [Required]
    [StringLength(200)]
    public string LastName { get; set; } = default!;

    [EmailAddress]
    public string Email { get; set; } = default!;

    [StringLength(20)]
    [Required]
    public string DocumentNumber { get; set; } = default!;

    

    public int Age { get; set; }

    [Required]
    public string Password { get; set; } = default!;

    [Compare(nameof(Password), ErrorMessage = "The passwords must match.")]
    public string ConfirmPassword { get; set; } = default!;
}
