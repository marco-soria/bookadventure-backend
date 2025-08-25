using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;

namespace BookAdventure.Services.Interfaces;

public interface IGenreService
{
    Task<BaseResponseGeneric<List<GenreResponseDto>>> GetAsync();
    Task<BaseResponseGeneric<GenreResponseDto>> GetAsync(int id);
    Task<BaseResponseGeneric<int>> AddAsync(GenreRequestDto request);
    Task<BaseResponse> UpdateAsync(int id, GenreUpdateRequestDto request);
    Task<BaseResponse> DeleteAsync(int id);
}