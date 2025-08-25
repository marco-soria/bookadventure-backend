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
    private readonly IFileStorage _fileStorage;
    private readonly IMapper _mapper;

    public BookService(
        IBookRepository bookRepository, 
        IGenreRepository genreRepository, 
        IFileStorage fileStorage,
        IMapper mapper)
    {
        _bookRepository = bookRepository;
        _genreRepository = genreRepository;
        _fileStorage = fileStorage;
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

            // Handle file upload if provided
            if (request.ImageFile != null)
            {
                try
                {
                    var imagePath = await _fileStorage.SaveFileAsync(request.ImageFile, "books");
                    book.ImageUrl = _fileStorage.GetFileUrl(imagePath);
                }
                catch (Exception ex)
                {
                    return new BaseResponseGeneric<int>
                    {
                        Success = false,
                        ErrorMessage = $"Error uploading image: {ex.Message}"
                    };
                }
            }
            
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

    public async Task<BaseResponse> UpdateAsync(int id, BookRequestDto request)
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

            // Validate genre exists
            var genre = await _genreRepository.GetByIdAsync(request.GenreId);
            if (genre == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Genre not found"
                };
            }

            // Store old image URL for deletion if new file is uploaded
            var oldImageUrl = existingBook.ImageUrl;

            _mapper.Map(request, existingBook);
            existingBook.IsAvailable = existingBook.Stock > 0;

            // Handle file upload if provided
            if (request.ImageFile != null)
            {
                try
                {
                    var imagePath = await _fileStorage.SaveFileAsync(request.ImageFile, "books");
                    existingBook.ImageUrl = _fileStorage.GetFileUrl(imagePath);

                    // Delete old image if it exists and is not a URL
                    if (!string.IsNullOrEmpty(oldImageUrl) && !oldImageUrl.StartsWith("http"))
                    {
                        await _fileStorage.DeleteFile(oldImageUrl, "books");
                    }
                }
                catch (Exception ex)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        ErrorMessage = $"Error uploading image: {ex.Message}"
                    };
                }
            }
            
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
}
