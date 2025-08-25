using BookAdventure.Entities;
using BookAdventure.Persistence;
using BookAdventure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAdventure.Repositories.Implementation;

public class GenreRepository : BaseRepository<Genre>, IGenreRepository
{
    public GenreRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Genre?> GetByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(g => g.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Genre>> SearchByNameAsync(string namePattern)
    {
        return await _dbSet
            .Where(g => g.Name.Contains(namePattern))
            .ToListAsync();
    }
}
