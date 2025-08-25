using BookAdventure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookAdventure.Persistence.Seeders;

public class RentalOrderSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public RentalOrderSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
        {
            if (!await context.RentalOrders.AnyAsync())
            {
                // Obtener customers y books existentes
                var customers = await context.Customers.ToListAsync();
                var books = await context.Books.Take(20).ToListAsync(); // Tomar algunos libros para alquilar

                if (customers.Any() && books.Any())
                {
                    var random = new Random();
                    var orders = new List<RentalOrder>();

                    // Crear órdenes de alquiler para cada customer
                    foreach (var customer in customers)
                    {
                        // Crear 2-3 órdenes por customer para tener datos variados
                        for (int i = 0; i < random.Next(2, 4); i++)
                        {
                            var orderDate = DateTime.UtcNow.AddDays(-random.Next(1, 90)); // Órdenes de los últimos 90 días
                            var rentalDays = random.Next(7, 30); // Entre 7 y 30 días de alquiler
                            var dueDate = orderDate.AddDays(rentalDays);

                            var order = new RentalOrder
                            {
                                OrderNumber = $"RO{DateTime.UtcNow:MMddHHmm}{customer.Id:D2}{i}",
                                CustomerId = customer.Id,
                                OrderDate = orderDate,
                                DueDate = dueDate,
                                ReturnDate = random.Next(1, 10) > 7 ? dueDate.AddDays(random.Next(-3, 5)) : null, // 70% devueltos
                                OrderStatus = DetermineOrderStatus(orderDate, dueDate)
                            };

                            orders.Add(order);
                        }
                    }

                    await context.RentalOrders.AddRangeAsync(orders);
                    await context.SaveChangesAsync();

                    // Crear detalles de las órdenes
                    await CreateOrderDetails(context, orders, books);
                }
            }
        }
    }

    private async Task CreateOrderDetails(ApplicationDbContext context, List<RentalOrder> orders, List<Book> books)
    {
        var random = new Random();
        var orderDetails = new List<RentalOrderDetail>();

        foreach (var order in orders)
        {
            // Cada orden tiene entre 1 y 3 libros diferentes
            var numBooks = random.Next(1, 4);
            var selectedBooks = books.OrderBy(x => random.Next()).Take(numBooks).ToList();

            foreach (var book in selectedBooks)
            {
                var quantity = random.Next(1, 3); // 1 o 2 copias del mismo libro
                var rentalDays = (order.DueDate - order.OrderDate).Days;

                var detail = new RentalOrderDetail
                {
                    RentalOrderId = order.Id,
                    BookId = book.Id,
                    Quantity = quantity,
                    RentalDays = rentalDays
                };

                orderDetails.Add(detail);
            }
        }

        await context.RentalOrderDetails.AddRangeAsync(orderDetails);
        await context.SaveChangesAsync();
    }

    private OrderStatus DetermineOrderStatus(DateTime orderDate, DateTime dueDate)
    {
        var now = DateTime.UtcNow;
        
        if (now < dueDate)
            return OrderStatus.Active;
        else if (now <= dueDate.AddDays(7)) // Gracia de 7 días
            return OrderStatus.Returned;
        else
            return OrderStatus.Overdue;
    }
}
