using BookAdventure.Entities;

namespace BookAdventure.Repositories.Interfaces;

public interface IBookRepository : IBaseRepository<Book>
{
    Task<IEnumerable<Book>> SearchByTitleAsync(string title);
    Task<IEnumerable<Book>> GetByGenreAsync(int genreId);
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
    Task<bool> IsAvailableAsync(int bookId);
}
