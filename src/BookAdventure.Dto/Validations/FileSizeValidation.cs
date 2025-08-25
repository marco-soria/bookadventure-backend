using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Validations;

public class FileSizeValidation : ValidationAttribute
{
    private readonly int maxSizeInMegabytes;

    public FileSizeValidation(int MaxSizeInMegabytes)
    {
        maxSizeInMegabytes = MaxSizeInMegabytes;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        IFormFile? formfile = value as IFormFile;

        if (formfile is null)
            return ValidationResult.Success;

        if (formfile.Length > maxSizeInMegabytes * 1024 * 1024)
            return new ValidationResult($"File size must not exceed {maxSizeInMegabytes} mb.");

        return ValidationResult.Success;
    }
}
