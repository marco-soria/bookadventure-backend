using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Services.Interfaces.Base;

namespace BookAdventure.Services.Interfaces;

public interface IGenreService : IBaseService<Genre>
{
    Task<BaseResponseGeneric<List<GenreResponseDto>>> GetAsync();
    Task<BaseResponseGeneric<GenreResponseDto>> GetAsync(int id);
    Task<BaseResponseGeneric<int>> AddAsync(GenreRequestDto request);
    Task<BaseResponse> UpdateAsync(int id, GenreUpdateRequestDto request);
    Task<BaseResponse> DeleteAsync(int id);
    
    // Métodos para manejo de eliminación lógica
    Task<BaseResponseGeneric<List<GenreResponseDto>>> GetDeletedGenresAsync(PaginationDto pagination);
    Task<BaseResponse> RestoreGenreAsync(int id);
    
    // Método para admin panel
    Task<BaseResponseGeneric<List<GenreResponseDto>>> GetAllGenresForAdminAsync(PaginationDto pagination);
}