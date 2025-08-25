using BookAdventure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookAdventure.Persistence.Seeders;

public class BookSeeder
{
    private readonly ApplicationDbContext _context;

    public BookSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (!await _context.Books.AnyAsync())
        {
            // Get genres from database
            var fiction = await _context.Genres.FirstAsync(g => g.Name == "Fiction");
            var nonFiction = await _context.Genres.FirstAsync(g => g.Name == "Non-Fiction");
            var scienceFiction = await _context.Genres.FirstAsync(g => g.Name == "Science Fiction");
            var fantasy = await _context.Genres.FirstAsync(g => g.Name == "Fantasy");
            var mystery = await _context.Genres.FirstAsync(g => g.Name == "Mystery");
            var romance = await _context.Genres.FirstAsync(g => g.Name == "Romance");
            var thriller = await _context.Genres.FirstAsync(g => g.Name == "Thriller");
            var horror = await _context.Genres.FirstAsync(g => g.Name == "Horror");
            var biography = await _context.Genres.FirstAsync(g => g.Name == "Biography");
            var history = await _context.Genres.FirstAsync(g => g.Name == "History");
            var selfHelp = await _context.Genres.FirstAsync(g => g.Name == "Self-Help");
            var technology = await _context.Genres.FirstAsync(g => g.Name == "Technology");

                var books = new List<Book>
                {
                    // Fiction Books
                    new() { Title = "To Kill a Mockingbird", Author = "Harper Lee", ISBN = "9780061120084", Description = "A classic novel about racial injustice and childhood innocence", Stock = 25, GenreId = fiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780061120084-L.jpg", IsAvailable = true },
                    new() { Title = "1984", Author = "George Orwell", ISBN = "9780451524935", Description = "A dystopian social science fiction novel", Stock = 30, GenreId = fiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780451524935-L.jpg", IsAvailable = true },
                    new() { Title = "Pride and Prejudice", Author = "Jane Austen", ISBN = "9780141439518", Description = "A romantic novel of manners", Stock = 20, GenreId = fiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780141439518-L.jpg", IsAvailable = true },
                    new() { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", ISBN = "9780743273565", Description = "A critique of the American Dream", Stock = 18, GenreId = fiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780743273565-L.jpg", IsAvailable = true },

                    // Non-Fiction Books
                    new() { Title = "Sapiens", Author = "Yuval Noah Harari", ISBN = "9780062316097", Description = "A brief history of humankind", Stock = 22, GenreId = nonFiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780062316097-L.jpg", IsAvailable = true },
                    new() { Title = "Educated", Author = "Tara Westover", ISBN = "9780399590504", Description = "A memoir about education and family", Stock = 19, GenreId = nonFiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780399590504-L.jpg", IsAvailable = true },
                    new() { Title = "Atomic Habits", Author = "James Clear", ISBN = "9780735211292", Description = "An easy and proven way to build good habits", Stock = 35, GenreId = nonFiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780735211292-L.jpg", IsAvailable = true },
                    new() { Title = "The Immortal Life of Henrietta Lacks", Author = "Rebecca Skloot", ISBN = "9781400052189", Description = "The story of how one woman's cells changed medicine", Stock = 16, GenreId = nonFiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9781400052189-L.jpg", IsAvailable = true },

                    // Science Fiction Books
                    new() { Title = "Dune", Author = "Frank Herbert", ISBN = "9780441172719", Description = "Epic science fiction novel set in the distant future", Stock = 28, GenreId = scienceFiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780441172719-L.jpg", IsAvailable = true },
                    new() { Title = "Foundation", Author = "Isaac Asimov", ISBN = "9780553293357", Description = "First book in the Foundation series", Stock = 24, GenreId = scienceFiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780553293357-L.jpg", IsAvailable = true },
                    new() { Title = "Neuromancer", Author = "William Gibson", ISBN = "9780441569595", Description = "Groundbreaking cyberpunk novel", Stock = 21, GenreId = scienceFiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780441569595-L.jpg", IsAvailable = true },
                    new() { Title = "The Martian", Author = "Andy Weir", ISBN = "9780553418026", Description = "A stranded astronaut's fight for survival on Mars", Stock = 32, GenreId = scienceFiction.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780553418026-L.jpg", IsAvailable = true },

                    // Fantasy Books
                    new() { Title = "The Lord of the Rings", Author = "J.R.R. Tolkien", ISBN = "9780544003415", Description = "Epic high fantasy adventure", Stock = 15, GenreId = fantasy.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780544003415-L.jpg", IsAvailable = true },
                    new() { Title = "Harry Potter and the Sorcerer's Stone", Author = "J.K. Rowling", ISBN = "9780439708180", Description = "The beginning of the magical journey", Stock = 40, GenreId = fantasy.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780439708180-L.jpg", IsAvailable = true },
                    new() { Title = "A Game of Thrones", Author = "George R.R. Martin", ISBN = "9780553593716", Description = "Political intrigue in a fantasy world", Stock = 26, GenreId = fantasy.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780553593716-L.jpg", IsAvailable = true },
                    new() { Title = "The Name of the Wind", Author = "Patrick Rothfuss", ISBN = "9780756404079", Description = "The story of Kvothe, a legendary figure", Stock = 23, GenreId = fantasy.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780756404079-L.jpg", IsAvailable = true },

                    // Mystery Books
                    new() { Title = "The Girl with the Dragon Tattoo", Author = "Stieg Larsson", ISBN = "9780307454546", Description = "A gripping thriller about murder and corruption", Stock = 27, GenreId = mystery.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780307454546-L.jpg", IsAvailable = true },
                    new() { Title = "Gone Girl", Author = "Gillian Flynn", ISBN = "9780307588371", Description = "A psychological thriller about a missing wife", Stock = 29, GenreId = mystery.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780307588371-L.jpg", IsAvailable = true },
                    new() { Title = "The Big Sleep", Author = "Raymond Chandler", ISBN = "9780394758282", Description = "Classic hard-boiled detective fiction", Stock = 18, GenreId = mystery.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780394758282-L.jpg", IsAvailable = true },
                    new() { Title = "In the Woods", Author = "Tana French", ISBN = "9780143113492", Description = "Atmospheric psychological mystery", Stock = 22, GenreId = mystery.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780143113492-L.jpg", IsAvailable = true },

                    // Romance Books
                    new() { Title = "Outlander", Author = "Diana Gabaldon", ISBN = "9780440212560", Description = "Time-traveling romance adventure", Stock = 31, GenreId = romance.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780440212560-L.jpg", IsAvailable = true },
                    new() { Title = "Me Before You", Author = "Jojo Moyes", ISBN = "9780670026609", Description = "A love story that will break your heart", Stock = 25, GenreId = romance.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780670026609-L.jpg", IsAvailable = true },
                    new() { Title = "The Notebook", Author = "Nicholas Sparks", ISBN = "9780446605236", Description = "A timeless love story", Stock = 33, GenreId = romance.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780446605236-L.jpg", IsAvailable = true },
                    new() { Title = "Red, White & Royal Blue", Author = "Casey McQuiston", ISBN = "9781250316776", Description = "A romantic comedy about politics and love", Stock = 28, GenreId = romance.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9781250316776-L.jpg", IsAvailable = true },

                    // Thriller Books
                    new() { Title = "The Silence of the Lambs", Author = "Thomas Harris", ISBN = "9780312924584", Description = "Psychological horror thriller", Stock = 20, GenreId = thriller.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780312924584-L.jpg", IsAvailable = true },
                    new() { Title = "The Da Vinci Code", Author = "Dan Brown", ISBN = "9780307474278", Description = "A symbologist's quest for ancient secrets", Stock = 24, GenreId = thriller.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780307474278-L.jpg", IsAvailable = true },
                    new() { Title = "The Bourne Identity", Author = "Robert Ludlum", ISBN = "9780553260113", Description = "A man with no memory and deadly skills", Stock = 19, GenreId = thriller.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780553260113-L.jpg", IsAvailable = true },
                    new() { Title = "The Girl on the Train", Author = "Paula Hawkins", ISBN = "9781594634024", Description = "A psychological thriller about memory and truth", Stock = 26, GenreId = thriller.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9781594634024-L.jpg", IsAvailable = true },

                    // Horror Books
                    new() { Title = "The Shining", Author = "Stephen King", ISBN = "9780307743657", Description = "A family's descent into madness", Stock = 22, GenreId = horror.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780307743657-L.jpg", IsAvailable = true },
                    new() { Title = "Dracula", Author = "Bram Stoker", ISBN = "9780141439846", Description = "The classic vampire novel", Stock = 18, GenreId = horror.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780141439846-L.jpg", IsAvailable = true },
                    new() { Title = "Frankenstein", Author = "Mary Shelley", ISBN = "9780141439471", Description = "The modern Prometheus", Stock = 21, GenreId = horror.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780141439471-L.jpg", IsAvailable = true },
                    new() { Title = "The Exorcist", Author = "William Peter Blatty", ISBN = "9780062094360", Description = "A terrifying tale of demonic possession", Stock = 17, GenreId = horror.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780062094360-L.jpg", IsAvailable = true },

                    // Biography Books
                    new() { Title = "Steve Jobs", Author = "Walter Isaacson", ISBN = "9781451648539", Description = "The exclusive biography of Apple's co-founder", Stock = 23, GenreId = biography.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9781451648539-L.jpg", IsAvailable = true },
                    new() { Title = "Becoming", Author = "Michelle Obama", ISBN = "9781524763138", Description = "The intimate memoir of the former First Lady", Stock = 35, GenreId = biography.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9781524763138-L.jpg", IsAvailable = true },
                    new() { Title = "Long Walk to Freedom", Author = "Nelson Mandela", ISBN = "9780316548182", Description = "The autobiography of Nelson Mandela", Stock = 20, GenreId = biography.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780316548182-L.jpg", IsAvailable = true },
                    new() { Title = "The Diary of a Young Girl", Author = "Anne Frank", ISBN = "9780553296983", Description = "The moving diary of Anne Frank", Stock = 28, GenreId = biography.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780553296983-L.jpg", IsAvailable = true },

                    // History Books
                    new() { Title = "A People's History of the United States", Author = "Howard Zinn", ISBN = "9780062397348", Description = "American history from the bottom up", Stock = 19, GenreId = history.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780062397348-L.jpg", IsAvailable = true },
                    new() { Title = "The Guns of August", Author = "Barbara Tuchman", ISBN = "9780345476098", Description = "The outbreak of World War I", Stock = 16, GenreId = history.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780345476098-L.jpg", IsAvailable = true },
                    new() { Title = "1776", Author = "David McCullough", ISBN = "9780743226721", Description = "The pivotal year of American independence", Stock = 22, GenreId = history.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780743226721-L.jpg", IsAvailable = true },
                    new() { Title = "The Rise and Fall of the Third Reich", Author = "William L. Shirer", ISBN = "9781451651683", Description = "A history of Nazi Germany", Stock = 14, GenreId = history.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9781451651683-L.jpg", IsAvailable = true },

                    // Self-Help Books
                    new() { Title = "Think and Grow Rich", Author = "Napoleon Hill", ISBN = "9781585424337", Description = "The classic guide to wealth and success", Stock = 30, GenreId = selfHelp.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9781585424337-L.jpg", IsAvailable = true },
                    new() { Title = "How to Win Friends and Influence People", Author = "Dale Carnegie", ISBN = "9780671027032", Description = "Timeless advice for building relationships", Stock = 32, GenreId = selfHelp.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780671027032-L.jpg", IsAvailable = true },
                    new() { Title = "The 7 Habits of Highly Effective People", Author = "Stephen R. Covey", ISBN = "9781451639619", Description = "Powerful lessons in personal change", Stock = 27, GenreId = selfHelp.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9781451639619-L.jpg", IsAvailable = true },
                    new() { Title = "Mindset", Author = "Carol S. Dweck", ISBN = "9780345472328", Description = "The new psychology of success", Stock = 25, GenreId = selfHelp.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780345472328-L.jpg", IsAvailable = true },

                    // Technology Books
                    new() { Title = "Clean Code", Author = "Robert C. Martin", ISBN = "9780132350884", Description = "A handbook of agile software craftsmanship", Stock = 18, GenreId = technology.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780132350884-L.jpg", IsAvailable = true },
                    new() { Title = "The Pragmatic Programmer", Author = "David Thomas", ISBN = "9780201616224", Description = "From journeyman to master", Stock = 21, GenreId = technology.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780201616224-L.jpg", IsAvailable = true },
                    new() { Title = "Design Patterns", Author = "Gang of Four", ISBN = "9780201633612", Description = "Elements of reusable object-oriented software", Stock = 15, GenreId = technology.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9780201633612-L.jpg", IsAvailable = true },
                    new() { Title = "You Don't Know JS", Author = "Kyle Simpson", ISBN = "9781491924464", Description = "Up and going with JavaScript", Stock = 24, GenreId = technology.Id, ImageUrl = "https://covers.openlibrary.org/b/isbn/9781491924464-L.jpg", IsAvailable = true }
                };

                await _context.Books.AddRangeAsync(books);
                await _context.SaveChangesAsync();
            }
    }
}
