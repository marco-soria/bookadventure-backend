using AutoMapper;
using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Repositories.Interfaces;
using BookAdventure.Repositories.Utils;
using BookAdventure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAdventure.Services.Implementation;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public BookService(
        IBookRepository bookRepository, 
        IGenreRepository genreRepository, 
        IMapper mapper)
    {
        _bookRepository = bookRepository;
        _genreRepository = genreRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponseGeneric<List<BookResponseDto>>> GetAsync(PaginationDto pagination)
    {
        try
        {
            var books = await _bookRepository.Query()
                .Include(b => b.Genre)
                .Paginate(pagination)
                .ToListAsync();

            var totalRecords = await _bookRepository.CountAsync();
            
            // Manual mapping to avoid AutoMapper issues
            var response = books.Select(book => new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                Status = book.Status == EntityStatus.Active,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                GenreId = book.GenreId,
                GenreName = book.Genre?.Name ?? string.Empty
            }).ToList();

            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving books: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<BookResponseDto>> GetAsync(int id)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return new BaseResponseGeneric<BookResponseDto>
                {
                    Success = false,
                    ErrorMessage = "Book not found"
                };
            }

            // Manual mapping to avoid AutoMapper issues
            var response = new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                Status = book.Status == EntityStatus.Active,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                GenreId = book.GenreId,
                GenreName = book.Genre?.Name ?? string.Empty
            };

            return new BaseResponseGeneric<BookResponseDto>
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<BookResponseDto>
            {
                Success = false,
                ErrorMessage = $"Error retrieving book: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<int>> AddAsync(BookRequestDto request)
    {
        try
        {
            // Validate genre exists
            var genre = await _genreRepository.GetByIdAsync(request.GenreId);
            if (genre == null)
            {
                return new BaseResponseGeneric<int>
                {
                    Success = false,
                    ErrorMessage = "Genre not found"
                };
            }

            var book = _mapper.Map<Book>(request);
            book.IsAvailable = book.Stock > 0;
            
            var createdBook = await _bookRepository.CreateAsync(book);

            return new BaseResponseGeneric<int>
            {
                Success = true,
                Data = createdBook.Id
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<int>
            {
                Success = false,
                ErrorMessage = $"Error creating book: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> UpdateAsync(int id, BookUpdateRequestDto request)
    {
        try
        {
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Book not found"
                };
            }

            // Validate genre exists if provided
            if (request.GenreId.HasValue)
            {
                var genre = await _genreRepository.GetByIdAsync(request.GenreId.Value);
                if (genre == null)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        ErrorMessage = "Genre not found"
                    };
                }
                existingBook.GenreId = request.GenreId.Value;
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(request.Title))
                existingBook.Title = request.Title;
            
            if (!string.IsNullOrEmpty(request.Author))
                existingBook.Author = request.Author;
            
            if (!string.IsNullOrEmpty(request.ISBN))
                existingBook.ISBN = request.ISBN;
            
            if (!string.IsNullOrEmpty(request.Description))
                existingBook.Description = request.Description;
            
            if (request.Stock.HasValue)
                existingBook.Stock = request.Stock.Value;
            
            if (!string.IsNullOrEmpty(request.ImageUrl))
                existingBook.ImageUrl = request.ImageUrl;
            
            if (request.IsAvailable.HasValue)
                existingBook.IsAvailable = request.IsAvailable.Value;
            else if (request.Stock.HasValue)
                existingBook.IsAvailable = existingBook.Stock > 0;
            
            await _bookRepository.UpdateAsync(existingBook);

            return new BaseResponse { Success = true };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error updating book: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> DeleteAsync(int id)
    {
        try
        {
            var success = await _bookRepository.SoftDeleteAsync(id);

            if (!success)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Book not found"
                };
            }

            return new BaseResponse { Success = true };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error deleting book: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<List<BookResponseDto>>> SearchAsync(string title)
    {
        try
        {
            var books = await _bookRepository.SearchByTitleAsync(title);
            
            // Manual mapping to avoid AutoMapper issues
            var response = books.Select(book => new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                Status = book.Status == EntityStatus.Active,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                GenreId = book.GenreId,
                GenreName = book.Genre?.Name ?? string.Empty
            }).ToList();

            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error searching books: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<List<BookResponseDto>>> GetByGenreAsync(int genreId, PaginationDto pagination)
    {
        try
        {
            var books = await _bookRepository.Query()
                .Where(b => b.GenreId == genreId)
                .Include(b => b.Genre)
                .Paginate(pagination)
                .ToListAsync();

            var totalRecords = await _bookRepository.Query()
                .Where(b => b.GenreId == genreId)
                .CountAsync();

            // Manual mapping to avoid AutoMapper issues
            var response = books.Select(book => new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                Status = book.Status == EntityStatus.Active,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                GenreId = book.GenreId,
                GenreName = book.Genre?.Name ?? string.Empty
            }).ToList();

            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving books by genre: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<List<BookResponseDto>>> GetByGenreNameAsync(string genreName, PaginationDto pagination)
    {
        try
        {
            // First, find the genre by name
            var genre = await _genreRepository.GetByNameAsync(genreName);
            if (genre == null)
            {
                return new BaseResponseGeneric<List<BookResponseDto>>
                {
                    Success = false,
                    ErrorMessage = $"Genre '{genreName}' not found"
                };
            }

            var books = await _bookRepository.Query()
                .Where(b => b.GenreId == genre.Id)
                .Include(b => b.Genre)
                .Paginate(pagination)
                .ToListAsync();

            var totalRecords = await _bookRepository.Query()
                .Where(b => b.GenreId == genre.Id)
                .CountAsync();

            // Manual mapping to avoid AutoMapper issues
            var response = books.Select(book => new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                Status = book.Status == EntityStatus.Active,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                GenreId = book.GenreId,
                GenreName = book.Genre?.Name ?? string.Empty
            }).ToList();

            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving books by genre name: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<List<BookResponseDto>>> GetBooksWithFiltersAsync(BookSearchDto searchFilters)
    {
        try
        {
            var query = _bookRepository.Query()
                .Include(b => b.Genre)
                .Where(b => b.Status == EntityStatus.Active);

            // Apply filters
            if (searchFilters.GenreId.HasValue)
            {
                query = query.Where(b => b.GenreId == searchFilters.GenreId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchFilters.Author))
            {
                query = query.Where(b => b.Author.ToLower().Contains(searchFilters.Author.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(searchFilters.Search))
            {
                query = query.Where(b => 
                    b.Title.ToLower().Contains(searchFilters.Search.ToLower()) ||
                    b.Author.ToLower().Contains(searchFilters.Search.ToLower()) ||
                    (b.Description != null && b.Description.ToLower().Contains(searchFilters.Search.ToLower())));
            }

            if (searchFilters.InStock.HasValue)
            {
                if (searchFilters.InStock.Value)
                {
                    query = query.Where(b => b.Stock > 0);
                }
                else
                {
                    query = query.Where(b => b.Stock == 0);
                }
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(searchFilters.SortBy))
            {
                switch (searchFilters.SortBy.ToLower())
                {
                    case "title":
                        query = searchFilters.SortDescending 
                            ? query.OrderByDescending(b => b.Title)
                            : query.OrderBy(b => b.Title);
                        break;
                    case "author":
                        query = searchFilters.SortDescending 
                            ? query.OrderByDescending(b => b.Author)
                            : query.OrderBy(b => b.Author);
                        break;
                    case "genre":
                        query = searchFilters.SortDescending 
                            ? query.OrderByDescending(b => b.Genre.Name)
                            : query.OrderBy(b => b.Genre.Name);
                        break;
                    case "stock":
                        query = searchFilters.SortDescending 
                            ? query.OrderByDescending(b => b.Stock)
                            : query.OrderBy(b => b.Stock);
                        break;
                    case "createdat":
                        query = searchFilters.SortDescending 
                            ? query.OrderByDescending(b => b.CreatedAt)
                            : query.OrderBy(b => b.CreatedAt);
                        break;
                    default:
                        // Default sort by title alphabetically
                        query = query.OrderBy(b => b.Title);
                        break;
                }
            }
            else
            {
                // Default sort by title alphabetically
                query = query.OrderBy(b => b.Title);
            }

            // Get total count before pagination
            var totalRecords = await query.CountAsync();

            // Apply pagination
            var books = await query
                .Skip((searchFilters.Page - 1) * searchFilters.RecordsPerPage)
                .Take(searchFilters.RecordsPerPage)
                .ToListAsync();

            // Manual mapping to avoid AutoMapper issues
            var response = books.Select(book => new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                Status = book.Status == EntityStatus.Active,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                GenreId = book.GenreId,
                GenreName = book.Genre?.Name ?? string.Empty
            }).ToList();

            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving books with filters: {ex.Message}"
            };
        }
    }

    // Base service implementations
    public async Task<IEnumerable<Book>> GetAllAsync() => await _bookRepository.GetAllAsync();
    public async Task<IEnumerable<Book>> GetAllIncludingDeletedAsync() => await _bookRepository.GetAllIncludingDeletedAsync();
    public async Task<Book?> GetByIdAsync(int id) => await _bookRepository.GetByIdAsync(id);
    public async Task<Book?> GetByIdIncludingDeletedAsync(int id) => await _bookRepository.GetByIdIncludingDeletedAsync(id);
    public async Task<IEnumerable<Book>> FindAsync(System.Linq.Expressions.Expression<Func<Book, bool>> predicate) => await _bookRepository.FindAsync(predicate);
    public async Task<Book> CreateAsync(Book entity) => await _bookRepository.CreateAsync(entity);
    public async Task<Book?> UpdateAsync(Book entity) => await _bookRepository.UpdateAsync(entity);
    public async Task<bool> SoftDeleteAsync(int id) => await _bookRepository.SoftDeleteAsync(id);
    public async Task<bool> RestoreAsync(int id) => await _bookRepository.RestoreAsync(id);
    public async Task<bool> HardDeleteAsync(int id) => await _bookRepository.HardDeleteAsync(id);
    public async Task<bool> ExistsAsync(int id) => await _bookRepository.ExistsAsync(id);
    public async Task<int> CountAsync() => await _bookRepository.CountAsync();
    public async Task<int> CountIncludingDeletedAsync() => await _bookRepository.CountIncludingDeletedAsync();

    /// <summary>
    /// Obtiene todos los libros eliminados lógicamente con paginación
    /// </summary>
    public async Task<BaseResponseGeneric<List<BookResponseDto>>> GetDeletedBooksAsync(PaginationDto pagination)
    {
        try
        {
            var query = _bookRepository.QueryIncludingDeleted()
                .Where(b => b.Status == EntityStatus.Deleted)
                .Include(b => b.Genre);

            var totalRecords = await query.CountAsync();

            var books = await query
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage)
                .ToListAsync();

            var response = books.Select(book => new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                Status = book.Status == EntityStatus.Active,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                GenreId = book.GenreId,
                GenreName = book.Genre?.Name ?? string.Empty
            }).ToList();

            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving deleted books: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Restaura un libro eliminado lógicamente
    /// </summary>
    public async Task<BaseResponse> RestoreBookAsync(int id)
    {
        try
        {
            var book = await _bookRepository.GetByIdIncludingDeletedAsync(id);
            if (book == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Book not found"
                };
            }

            if (book.Status != EntityStatus.Deleted)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Book is not deleted"
                };
            }

            var result = await _bookRepository.RestoreAsync(id);
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
                ErrorMessage = "Failed to restore book"
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error restoring book: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtiene todos los libros para el panel de admin (incluyendo eliminados) con paginación
    /// </summary>
    public async Task<BaseResponseGeneric<List<BookResponseDto>>> GetAllBooksForAdminAsync(PaginationDto pagination)
    {
        try
        {
            var query = _bookRepository.QueryIncludingDeleted()
                .Include(b => b.Genre)
                .OrderBy(b => b.Id); // Orden consistente

            var totalRecords = await query.CountAsync();

            var books = await query
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage)
                .ToListAsync();

            var response = books.Select(book => new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                IsAvailable = book.IsAvailable,
                Status = book.Status == EntityStatus.Active, // true = activo, false = eliminado
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                GenreId = book.GenreId,
                GenreName = book.Genre?.Name ?? string.Empty
            }).ToList();

            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<List<BookResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving all books for admin: {ex.Message}"
            };
        }
    }
}
