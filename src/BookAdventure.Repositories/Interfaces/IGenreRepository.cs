using BookAdventure.Entities;

namespace BookAdventure.Repositories.Interfaces;

public interface IGenreRepository : IBaseRepository<Genre>
{
    Task<Genre?> GetByNameAsync(string name);
    Task<IEnumerable<Genre>> SearchByNameAsync(string namePattern);
}
