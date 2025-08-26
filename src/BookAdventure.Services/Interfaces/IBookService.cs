using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Services.Interfaces.Base;

namespace BookAdventure.Services.Interfaces;

public interface IBookService : IBaseService<Book>
{
    Task<BaseResponseGeneric<List<BookResponseDto>>> GetAsync(PaginationDto pagination);
    Task<BaseResponseGeneric<BookResponseDto>> GetAsync(int id);
    Task<BaseResponseGeneric<int>> AddAsync(BookRequestDto request);
    Task<BaseResponse> UpdateAsync(int id, BookUpdateRequestDto request);
    Task<BaseResponse> DeleteAsync(int id);
    Task<BaseResponseGeneric<List<BookResponseDto>>> SearchAsync(string title);
    Task<BaseResponseGeneric<List<BookResponseDto>>> GetByGenreAsync(int genreId, PaginationDto pagination);
    Task<BaseResponseGeneric<List<BookResponseDto>>> GetByGenreNameAsync(string genreName, PaginationDto pagination);
    Task<BaseResponseGeneric<List<BookResponseDto>>> GetBooksWithFiltersAsync(BookSearchDto searchFilters);
}
