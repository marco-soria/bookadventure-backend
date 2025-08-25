using BookAdventure.Entities;
using BookAdventure.Persistence;
using BookAdventure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAdventure.Services.Implementation;

public class RentalQueryService : IRentalQueryService
{
    private readonly ApplicationDbContext _context;

    public RentalQueryService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos los libros alquilados por un cliente dado su DNI
    /// Optimizado con índices para búsqueda rápida por DNI
    /// </summary>
    /// <param name="dni">DNI del cliente</param>
    /// <param name="includeReturned">Si incluir libros ya devueltos (default: true)</param>
    /// <param name="includeOverdue">Si incluir libros vencidos (default: true)</param>
    /// <returns>Lista de libros alquilados con información del alquiler</returns>
    public async Task<List<RentalBookInfo>> GetRentedBooksByDniAsync(string dni, bool includeReturned = true, bool includeOverdue = true)
    {
        var query = from customer in _context.Customers
                    join order in _context.RentalOrders on customer.Id equals order.CustomerId
                    join detail in _context.RentalOrderDetails on order.Id equals detail.RentalOrderId
                    join book in _context.Books on detail.BookId equals book.Id
                    join genre in _context.Genres on book.GenreId equals genre.Id
                    where customer.DNI == dni
                    select new
                    {
                        // Customer info
                        CustomerName = customer.FirstName + " " + customer.LastName,
                        CustomerDNI = customer.DNI,
                        CustomerEmail = customer.Email,
                        
                        // Order info
                        OrderNumber = order.OrderNumber,
                        OrderDate = order.OrderDate,
                        DueDate = order.DueDate,
                        ReturnDate = order.ReturnDate,
                        OrderStatus = order.OrderStatus,
                        
                        // Book info
                        BookId = book.Id,
                        BookTitle = book.Title,
                        BookAuthor = book.Author,
                        BookISBN = book.ISBN,
                        BookImageUrl = book.ImageUrl,
                        GenreName = genre.Name,
                        
                        // Rental details
                        Quantity = detail.Quantity,
                        RentalDays = detail.RentalDays
                    };

        // Aplicar filtros según parámetros
        if (!includeReturned)
        {
            query = query.Where(x => x.OrderStatus != OrderStatus.Returned);
        }
        
        if (!includeOverdue)
        {
            query = query.Where(x => x.OrderStatus != OrderStatus.Overdue);
        }

        var results = await query
            .OrderByDescending(x => x.OrderDate)
            .ThenBy(x => x.BookTitle)
            .ToListAsync();

        return results.Select(x => new RentalBookInfo
        {
            CustomerName = x.CustomerName,
            CustomerDNI = x.CustomerDNI,
            CustomerEmail = x.CustomerEmail,
            OrderNumber = x.OrderNumber,
            OrderDate = x.OrderDate,
            DueDate = x.DueDate,
            ReturnDate = x.ReturnDate,
            OrderStatus = x.OrderStatus,
            BookId = x.BookId,
            BookTitle = x.BookTitle,
            BookAuthor = x.BookAuthor,
            BookISBN = x.BookISBN,
            BookImageUrl = x.BookImageUrl,
            GenreName = x.GenreName,
            Quantity = x.Quantity,
            RentalDays = x.RentalDays,
            IsOverdue = x.OrderStatus == OrderStatus.Overdue,
            DaysOverdue = x.OrderStatus == OrderStatus.Overdue && x.ReturnDate == null 
                ? (int)(DateTime.UtcNow - x.DueDate).TotalDays 
                : 0
        }).ToList();
    }

    /// <summary>
    /// Obtiene resumen de alquileres por DNI
    /// </summary>
    public async Task<RentalSummary> GetRentalSummaryByDniAsync(string dni)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.DNI == dni);
        if (customer == null)
        {
            throw new ArgumentException($"Customer with DNI {dni} not found");
        }

        var rentals = await GetRentedBooksByDniAsync(dni);
        
        return new RentalSummary
        {
            CustomerName = customer.FullName,
            CustomerDNI = dni,
            CustomerEmail = customer.Email,
            TotalRentals = rentals.Count,
            ActiveRentals = rentals.Count(r => r.OrderStatus == OrderStatus.Active),
            ReturnedRentals = rentals.Count(r => r.OrderStatus == OrderStatus.Returned),
            OverdueRentals = rentals.Count(r => r.OrderStatus == OrderStatus.Overdue),
            LastRentalDate = rentals.Max(r => r.OrderDate),
            FavoriteGenres = rentals
                .GroupBy(r => r.GenreName)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => new GenreCount { Genre = g.Key, Count = g.Count() })
                .ToList()
        };
    }

    /// <summary>
    /// Busca customers por DNI (útil para autocompletado)
    /// </summary>
    public async Task<List<CustomerBasicInfo>> SearchCustomersByDniAsync(string dniPrefix)
    {
        return await _context.Customers
            .Where(c => c.DNI.StartsWith(dniPrefix))
            .Select(c => new CustomerBasicInfo
            {
                DNI = c.DNI,
                FullName = c.FirstName + " " + c.LastName,
                Email = c.Email
            })
            .OrderBy(c => c.DNI)
            .Take(10)
            .ToListAsync();
    }
}

/// <summary>
/// Información detallada de un libro alquilado
/// </summary>
public class RentalBookInfo
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerDNI { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public OrderStatus OrderStatus { get; set; }
    
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public string BookISBN { get; set; } = string.Empty;
    public string BookImageUrl { get; set; } = string.Empty;
    public string GenreName { get; set; } = string.Empty;
    
    public int Quantity { get; set; }
    public int RentalDays { get; set; }
    
    public bool IsOverdue { get; set; }
    public int DaysOverdue { get; set; }
    public bool IsReturned => ReturnDate.HasValue;
}

/// <summary>
/// Resumen de alquileres de un cliente
/// </summary>
public class RentalSummary
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerDNI { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public int TotalRentals { get; set; }
    public int ActiveRentals { get; set; }
    public int ReturnedRentals { get; set; }
    public int OverdueRentals { get; set; }
    public DateTime? LastRentalDate { get; set; }
    public List<GenreCount> FavoriteGenres { get; set; } = new();
}

public class GenreCount
{
    public string Genre { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class CustomerBasicInfo
{
    public string DNI { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
