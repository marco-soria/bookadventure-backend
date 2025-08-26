using BookAdventure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookAdventure.Persistence.Seeders;

public class RentalOrderSeeder
{
    private readonly ApplicationDbContext _context;

    public RentalOrderSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (!await _context.RentalOrders.AnyAsync())
        {
            // Get existing customers and books
            var customers = await _context.Customers.Take(3).ToListAsync();
            var books = await _context.Books.Take(10).ToListAsync();

            if (customers.Any() && books.Any())
            {
                var random = new Random();
                var orders = new List<RentalOrder>();

                // Create rental orders for each customer
                foreach (var customer in customers)
                {
                    // Create 1-2 orders per customer
                    for (int i = 0; i < random.Next(1, 3); i++)
                    {
                        var rentalDate = DateTime.UtcNow.AddDays(-random.Next(1, 30)); // Orders from last 30 days
                        var rentalDays = random.Next(7, 21); // 7-21 days rental period
                        var dueDate = rentalDate.AddDays(rentalDays);

                        var order = new RentalOrder
                        {
                            OrderNumber = $"RO{DateTime.UtcNow:yyyyMMdd}{customer.Id:D3}{i:D2}",
                            CustomerId = customer.Id,
                            OrderDate = rentalDate,
                            DueDate = dueDate,
                            OrderStatus = OrderStatus.Active,
                            Status = EntityStatus.Active,
                            CreatedAt = rentalDate,
                            UpdatedAt = rentalDate,
                            RentalOrderDetails = new List<RentalOrderDetail>()
                        };

                        // Add 1-3 books to each order
                        var numBooks = random.Next(1, 4);
                        var selectedBooks = books.OrderBy(x => random.Next()).Take(numBooks).ToList();

                        foreach (var book in selectedBooks)
                        {
                            var quantity = 1; // For simplicity, always 1 copy
                            var detailRentalDays = (dueDate - rentalDate).Days;
                            var isReturned = random.Next(1, 10) > 3; // 70% returned
                            var returnDate = isReturned ? dueDate.AddDays(random.Next(-2, 3)) : (DateTime?)null;

                            var detail = new RentalOrderDetail
                            {
                                BookId = book.Id,
                                Quantity = quantity,
                                RentalDays = detailRentalDays,
                                DueDate = dueDate,
                                ReturnDate = returnDate,
                                IsReturned = isReturned,
                                Status = EntityStatus.Active,
                                CreatedAt = rentalDate,
                                UpdatedAt = returnDate ?? rentalDate
                            };

                            order.RentalOrderDetails.Add(detail);
                        }
                        
                        // Update order status based on details
                        var allReturned = order.RentalOrderDetails.All(d => d.IsReturned);
                        var hasOverdue = order.RentalOrderDetails.Any(d => !d.IsReturned && d.DueDate < DateTime.UtcNow);
                        
                        if (allReturned)
                        {
                            order.OrderStatus = OrderStatus.Returned;
                        }
                        else if (hasOverdue)
                        {
                            order.OrderStatus = OrderStatus.Overdue;
                        }
                        else
                        {
                            order.OrderStatus = OrderStatus.Active;
                        }
                        
                        orders.Add(order);
                    }
                }

                await _context.RentalOrders.AddRangeAsync(orders);
                await _context.SaveChangesAsync();
            }
        }
    }
}
