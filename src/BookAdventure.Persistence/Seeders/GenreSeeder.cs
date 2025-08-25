using BookAdventure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookAdventure.Persistence.Seeders;

public class GenreSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public GenreSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
        {
            var listGenres = new List<Genre>
            {
                new Genre { Name = "Fiction" },
                new Genre { Name = "Non-Fiction" },
                new Genre { Name = "Science Fiction" },
                new Genre { Name = "Fantasy" },
                new Genre { Name = "Mystery" },
                new Genre { Name = "Romance" },
                new Genre { Name = "Thriller" },
                new Genre { Name = "Horror" },
                new Genre { Name = "Biography" },
                new Genre { Name = "History" },
                new Genre { Name = "Self-Help" },
                new Genre { Name = "Technology" }
            };

            var genreNamesToAdd = listGenres.Select(g => g.Name).ToHashSet();

            var existingGenreNames = await context.Set<Genre>()
                .Where(g => genreNamesToAdd.Contains(g.Name))
                .Select(g => g.Name)
                .ToListAsync();

            var genresToAdd = listGenres
                .Where(g => !existingGenreNames.Contains(g.Name))
                .ToList();

            if (genresToAdd.Any())
            {
                await context.Set<Genre>().AddRangeAsync(genresToAdd);
                await context.SaveChangesAsync();
            }
        }
    }
}
