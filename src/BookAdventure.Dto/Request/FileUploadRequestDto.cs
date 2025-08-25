using BookAdventure.Dto.Validations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class FileUploadRequestDto
{
    [Required(ErrorMessage = "File is required")]
    [FileTypeValidation(FileTypeGroup.Image)]
    [FileSizeValidation(5)] // 5MB max
    public IFormFile File { get; set; } = default!;

    [StringLength(100)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string Folder { get; set; } = "general";
}
