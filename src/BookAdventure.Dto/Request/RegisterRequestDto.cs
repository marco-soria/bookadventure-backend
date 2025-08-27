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

    [Required(ErrorMessage = "Age is required")]
    [Range(18, 120, ErrorMessage = "Age must be between 18 and 120 years")]
    public int Age { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    public string Password { get; set; } = default!;

    [Compare(nameof(Password), ErrorMessage = "The passwords must match.")]
    public string ConfirmPassword { get; set; } = default!;
}
