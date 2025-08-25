using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class PaginationDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;
    
    private int recordsPerPage = 10;
    private readonly int maxRecordsPerPage = 50;

    [Range(1, 100, ErrorMessage = "Records per page must be between 1 and 100")]
    public int RecordsPerPage
    {
        get
        {
            return recordsPerPage;
        }
        set
        {
            recordsPerPage = (value > maxRecordsPerPage) ? maxRecordsPerPage : value;
        }
    }
    
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class PaginatedResponseDto<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

public class BookSearchDto : PaginationDto
{
    public int? GenreId { get; set; }
    public string? Author { get; set; }
    public bool? InStock { get; set; }
}
