namespace BookAdventure.Dto.Response;

public class FileUploadResponseDto
{
    public string FileName { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public string FileUrl { get; set; } = default!;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = default!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
