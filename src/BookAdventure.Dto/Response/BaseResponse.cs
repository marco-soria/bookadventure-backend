namespace BookAdventure.Dto.Response;

public class BaseResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? TotalRecords { get; set; }
}
