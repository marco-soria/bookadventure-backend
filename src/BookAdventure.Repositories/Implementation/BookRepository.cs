using BookAdventure.Entities;
using BookAdventure.Persistence;
using BookAdventure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAdventure.Repositories.Implementation;

public class BookRepository : BaseRepository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Book>> SearchByTitleAsync(string title)
    {
        return await _dbSet
            .Where(b => b.Title.Contains(title))
            .Include(b => b.Genre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetByGenreAsync(int genreId)
    {
        return await _dbSet
            .Where(b => b.GenreId == genreId)
            .Include(b => b.Genre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
    {
        return await _dbSet
            .Where(b => b.IsAvailable)
            .Include(b => b.Genre)
            .ToListAsync();
    }

    public async Task<bool> IsAvailableAsync(int bookId)
    {
        var book = await _dbSet.FirstOrDefaultAsync(b => b.Id == bookId);
        return book?.IsAvailable ?? false;
    }

    public override async Task<Book?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Genre)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public override async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _dbSet
            .Include(b => b.Genre)
            .ToListAsync();
    }
}
