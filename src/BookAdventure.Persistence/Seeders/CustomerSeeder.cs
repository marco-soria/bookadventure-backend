using BookAdventure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookAdventure.Persistence.Seeders;

public class CustomerSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public CustomerSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<BookAdventureUserIdentity>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!await context.Customers.AnyAsync())
            {
                // Datos de usuarios/customers
                var customerData = new List<(string email, string firstName, string lastName, string dni, int age, string phone, string password)>
                {
                    ("john.doe@example.com", "John", "Doe", "12345678A", 28, "+1234567890", "Customer123!"),
                    ("jane.smith@example.com", "Jane", "Smith", "87654321B", 34, "+0987654321", "Customer123!"),
                    ("mike.johnson@example.com", "Mike", "Johnson", "11223344C", 42, "+1122334455", "Customer123!"),
                    ("sarah.williams@example.com", "Sarah", "Williams", "55667788D", 26, "+5566778899", "Customer123!"),
                    ("david.brown@example.com", "David", "Brown", "99887766E", 31, "+9988776655", "Customer123!")
                };

                foreach (var (email, firstName, lastName, dni, age, phone, password) in customerData)
                {
                    // 1. Crear usuario de Identity si no existe
                    var existingUser = await userManager.FindByEmailAsync(email);
                    BookAdventureUserIdentity user;
                    
                    if (existingUser == null)
                    {
                        user = new BookAdventureUserIdentity
                        {
                            UserName = email,
                            Email = email,
                            FirstName = firstName,
                            LastName = lastName,
                            DNI = dni,
                            Age = age,
                            PhoneNumber = phone,
                            EmailConfirmed = true
                        };

                        var result = await userManager.CreateAsync(user, password);
                        if (!result.Succeeded)
                        {
                            throw new Exception($"Failed to create user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }

                        // Asignar rol de Customer
                        await userManager.AddToRoleAsync(user, Constants.RoleCustomer);
                    }
                    else
                    {
                        user = existingUser;
                    }

                    // 2. Crear Customer entity vinculado al usuario
                    var existingCustomer = await context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
                    
                    if (existingCustomer == null)
                    {
                        var customer = new Customer
                        {
                            Email = email,
                            FirstName = firstName,
                            LastName = lastName,
                            DNI = dni,
                            Age = age,
                            PhoneNumber = phone,
                            UserId = user.Id // ✅ Vinculamos al usuario
                        };

                        await context.Customers.AddAsync(customer);
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
