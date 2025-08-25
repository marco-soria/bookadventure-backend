using System.ComponentModel.DataAnnotations;

namespace BookAdventure.Dto.Request;

public class PaginationDto
{
    private int page = 1;
    
    public int Page 
    { 
        get => page;
        set => page = value > 0 ? value : 1; // Default to 1 if invalid
    }
    
    private int recordsPerPage = 10;
    private readonly int maxRecordsPerPage = 50;

    public int RecordsPerPage
    {
        get
        {
            return recordsPerPage;
        }
        set
        {
            recordsPerPage = (value > maxRecordsPerPage) ? maxRecordsPerPage : value;
            if (recordsPerPage <= 0) recordsPerPage = 10; // Default to 10 if invalid
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
