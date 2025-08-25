using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;

namespace BookAdventure.Services.Interfaces;

public interface IGenreService
{
    Task<BaseResponseGeneric<ICollection<GenreResponseDto>>> GetAsync();
    Task<BaseResponseGeneric<GenreResponseDto>> GetAsync(int id);
    Task<BaseResponseGeneric<int>> AddAsync(GenreRequestDto request);
    Task<BaseResponse> UpdateAsync(int id, GenreRequestDto request);
    Task<BaseResponse> DeleteAsync(int id);
}