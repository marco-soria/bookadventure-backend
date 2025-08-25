using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class CustomerUpdateRequestDto
{
    [EmailAddress(ErrorMessage = "Must be a valid email")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string? Email { get; set; }
    
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string? FirstName { get; set; }
    
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string? LastName { get; set; }
    
    [StringLength(20, ErrorMessage = "DNI cannot exceed 20 characters")]
    public string? DNI { get; set; }
    
    [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
    public int? Age { get; set; }
    
    [Phone(ErrorMessage = "Must be a valid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }
}
