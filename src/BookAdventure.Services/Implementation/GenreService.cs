using AutoMapper;
using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Repositories.Interfaces;
using BookAdventure.Services.Interfaces;

namespace BookAdventure.Services.Implementation;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public GenreService(IGenreRepository genreRepository, IMapper mapper)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponseGeneric<ICollection<GenreResponseDto>>> GetAsync()
    {
        try
        {
            var genres = await _genreRepository.GetAllAsync();
            var response = _mapper.Map<ICollection<GenreResponseDto>>(genres);

            return new BaseResponseGeneric<ICollection<GenreResponseDto>>
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<ICollection<GenreResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving genres: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<GenreResponseDto>> GetAsync(int id)
    {
        try
        {
            var genre = await _genreRepository.GetByIdAsync(id);

            if (genre == null)
            {
                return new BaseResponseGeneric<GenreResponseDto>
                {
                    Success = false,
                    ErrorMessage = "Genre not found"
                };
            }

            var response = _mapper.Map<GenreResponseDto>(genre);

            return new BaseResponseGeneric<GenreResponseDto>
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<GenreResponseDto>
            {
                Success = false,
                ErrorMessage = $"Error retrieving genre: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<int>> AddAsync(GenreRequestDto request)
    {
        try
        {
            // Check if genre already exists
            var existingGenre = await _genreRepository.GetByNameAsync(request.Name);
            if (existingGenre != null)
            {
                return new BaseResponseGeneric<int>
                {
                    Success = false,
                    ErrorMessage = "Genre with this name already exists"
                };
            }

            var genre = _mapper.Map<Genre>(request);
            var createdGenre = await _genreRepository.CreateAsync(genre);

            return new BaseResponseGeneric<int>
            {
                Success = true,
                Data = createdGenre.Id
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<int>
            {
                Success = false,
                ErrorMessage = $"Error creating genre: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> UpdateAsync(int id, GenreRequestDto request)
    {
        try
        {
            var existingGenre = await _genreRepository.GetByIdAsync(id);
            if (existingGenre == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Genre not found"
                };
            }

            // Check if another genre with the same name exists
            var genreWithSameName = await _genreRepository.GetByNameAsync(request.Name);
            if (genreWithSameName != null && genreWithSameName.Id != id)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Another genre with this name already exists"
                };
            }

            _mapper.Map(request, existingGenre);
            await _genreRepository.UpdateAsync(existingGenre);

            return new BaseResponse { Success = true };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error updating genre: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> DeleteAsync(int id)
    {
        try
        {
            var success = await _genreRepository.SoftDeleteAsync(id);

            if (!success)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Genre not found"
                };
            }

            return new BaseResponse { Success = true };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error deleting genre: {ex.Message}"
            };
        }
    }
}
