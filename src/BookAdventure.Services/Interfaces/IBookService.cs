using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Services.Interfaces.Base;

namespace BookAdventure.Services.Interfaces;

public interface IBookService : IBaseService<Book>
{
    Task<BaseResponseGeneric<ICollection<BookResponseDto>>> GetAsync(PaginationDto pagination);
    Task<BaseResponseGeneric<BookResponseDto>> GetAsync(int id);
    Task<BaseResponseGeneric<int>> AddAsync(BookRequestDto request);
    Task<BaseResponse> UpdateAsync(int id, BookRequestDto request);
    Task<BaseResponse> DeleteAsync(int id);
    Task<BaseResponseGeneric<ICollection<BookResponseDto>>> SearchAsync(string title);
    Task<BaseResponseGeneric<ICollection<BookResponseDto>>> GetByGenreAsync(int genreId, PaginationDto pagination);
}
