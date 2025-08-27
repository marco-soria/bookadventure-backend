using AutoMapper;
using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Persistence;
using BookAdventure.Repositories.Interfaces;
using BookAdventure.Services.Base;
using BookAdventure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAdventure.Services.Implementation;

public class GenreService : BaseService<Genre>, IGenreService
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public GenreService(IGenreRepository genreRepository, IMapper mapper, ApplicationDbContext context) 
        : base(context)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponseGeneric<List<GenreResponseDto>>> GetAsync()
    {
        try
        {
            // Get genres with their books to calculate TotalBooks
            var genres = await _genreRepository.Query()
                .Include(g => g.Books.Where(b => b.Status == EntityStatus.Active))
                .Where(g => g.Status == EntityStatus.Active)
                .ToListAsync();
            
            // Manual mapping to avoid AutoMapper issues
            var response = genres.Select(genre => new GenreResponseDto
            {
                Id = genre.Id,
                Name = genre.Name,
                Status = genre.Status == EntityStatus.Active,
                CreatedAt = genre.CreatedAt,
                UpdatedAt = genre.UpdatedAt,
                TotalBooks = genre.Books?.Count ?? 0
            }).ToList();

            return new BaseResponseGeneric<List<GenreResponseDto>>
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<List<GenreResponseDto>>
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
            // Get genre with its books to calculate TotalBooks
            var genre = await _genreRepository.Query()
                .Include(g => g.Books.Where(b => b.Status == EntityStatus.Active))
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null)
            {
                return new BaseResponseGeneric<GenreResponseDto>
                {
                    Success = false,
                    ErrorMessage = "Genre not found"
                };
            }

            // Manual mapping to avoid AutoMapper issues
            var response = new GenreResponseDto
            {
                Id = genre.Id,
                Name = genre.Name,
                Status = genre.Status == EntityStatus.Active,
                CreatedAt = genre.CreatedAt,
                UpdatedAt = genre.UpdatedAt,
                TotalBooks = genre.Books?.Count ?? 0
            };

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

            // Manual mapping to avoid AutoMapper issues
            var genre = new Genre
            {
                Name = request.Name,
                Status = request.Status ? EntityStatus.Active : EntityStatus.Deleted
            };
            
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

    public async Task<BaseResponse> UpdateAsync(int id, GenreUpdateRequestDto request)
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

            // Check if another genre with the same name exists (only if name is provided)
            if (!string.IsNullOrEmpty(request.Name))
            {
                var genreWithSameName = await _genreRepository.GetByNameAsync(request.Name);
                if (genreWithSameName != null && genreWithSameName.Id != id)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        ErrorMessage = "Another genre with this name already exists"
                    };
                }
                existingGenre.Name = request.Name;
            }

            // Update status only if provided
            if (request.Status.HasValue)
            {
                existingGenre.Status = request.Status.Value ? EntityStatus.Active : EntityStatus.Deleted;
            }

            existingGenre.UpdatedAt = DateTime.UtcNow;
            
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

    /// <summary>
    /// Obtiene todos los géneros eliminados lógicamente con paginación
    /// </summary>
    public async Task<BaseResponseGeneric<List<GenreResponseDto>>> GetDeletedGenresAsync(PaginationDto pagination)
    {
        try
        {
            var query = _genreRepository.QueryIncludingDeleted()
                .Where(g => g.Status == EntityStatus.Deleted);

            var totalRecords = await query.CountAsync();

            var genres = await query
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage)
                .ToListAsync();

            // Manual mapping to avoid AutoMapper issues
            var response = genres.Select(genre => new GenreResponseDto
            {
                Id = genre.Id,
                Name = genre.Name,
                Status = false, // Always false for deleted entities
                CreatedAt = genre.CreatedAt,
                UpdatedAt = genre.UpdatedAt,
                TotalBooks = 0 // Don't count books for deleted genres
            }).ToList();

            return new BaseResponseGeneric<List<GenreResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<List<GenreResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving deleted genres: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Restaura un género eliminado lógicamente
    /// </summary>
    public async Task<BaseResponse> RestoreGenreAsync(int id)
    {
        try
        {
            var genre = await _genreRepository.GetByIdIncludingDeletedAsync(id);
            if (genre == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Genre not found"
                };
            }

            if (genre.Status != EntityStatus.Deleted)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Genre is not deleted"
                };
            }

            // Verificar si ya existe otro género activo con el mismo nombre
            var existingGenreWithSameName = await _genreRepository
                .Query()
                .FirstOrDefaultAsync(g => g.Name.ToLower() == genre.Name.ToLower());

            if (existingGenreWithSameName != null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Another active genre with the same name already exists"
                };
            }

            var result = await _genreRepository.RestoreAsync(id);
            if (result)
            {
                return new BaseResponse
                {
                    Success = true
                };
            }

            return new BaseResponse
            {
                Success = false,
                ErrorMessage = "Failed to restore genre"
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error restoring genre: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtiene todos los géneros para el panel de admin (incluyendo eliminados) con paginación
    /// </summary>
    public async Task<BaseResponseGeneric<List<GenreResponseDto>>> GetAllGenresForAdminAsync(PaginationDto pagination)
    {
        try
        {
            var query = _genreRepository.QueryIncludingDeleted()
                .Include(g => g.Books.Where(b => b.Status == EntityStatus.Active))
                .OrderBy(g => g.Id); // Orden consistente

            var totalRecords = await query.CountAsync();

            var genres = await query
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage)
                .ToListAsync();

            var response = genres.Select(genre => new GenreResponseDto
            {
                Id = genre.Id,
                Name = genre.Name,
                Status = genre.Status == EntityStatus.Active, // true = activo, false = eliminado
                CreatedAt = genre.CreatedAt,
                UpdatedAt = genre.UpdatedAt,
                TotalBooks = genre.Books?.Count ?? 0
            }).ToList();

            return new BaseResponseGeneric<List<GenreResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<List<GenreResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving all genres for admin: {ex.Message}"
            };
        }
    }
}
