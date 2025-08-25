using Microsoft.Extensions.DependencyInjection;

namespace BookAdventure.Persistence.Seeders;

/// <summary>
/// Seeder maestro que ejecuta todos los seeders en el orden correcto
/// </summary>
public class MasterSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public MasterSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Ejecuta todos los seeders en el orden correcto
    /// 1. UserDataSeeder - Roles y usuario admin
    /// 2. GenreSeeder - Géneros de libros
    /// 3. BookSeeder - Libros de muestra
    /// 4. CustomerSeeder - Usuarios customer e información de clientes
    /// 5. RentalOrderSeeder - Órdenes de alquiler de prueba
    /// </summary>
    public async Task SeedAllAsync()
    {
        try
        {
            Console.WriteLine("🌱 Initializing seeding process...");

            // Cada seeder en su propio scope para evitar problemas de contexto
            
            // 1. Roles y usuario administrador
            Console.WriteLine("📋 Creating roles and admin user...");
            using (var scope1 = _serviceProvider.CreateScope())
            {
                var userDataSeeder = new UserDataSeeder(scope1.ServiceProvider);
                await userDataSeeder.SeedAsync();
            }

            // 2. Géneros de libros
            Console.WriteLine("📚 Creating book genres...");
            using (var scope2 = _serviceProvider.CreateScope())
            {
                var genreSeeder = new GenreSeeder(scope2.ServiceProvider);
                await genreSeeder.SeedAsync();
            }

            // 3. Libros de muestra
            Console.WriteLine("📖 Creating sample books...");
            using (var scope3 = _serviceProvider.CreateScope())
            {
                var context = scope3.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var bookSeeder = new BookSeeder(context);
                await bookSeeder.SeedAsync();
            }

            // 4. Usuarios customer y entidades Customer
            Console.WriteLine("👥 Creating customer users...");
            using (var scope4 = _serviceProvider.CreateScope())
            {
                var customerSeeder = new CustomerSeeder(scope4.ServiceProvider);
                await customerSeeder.SeedAsync();
            }

            // 5. Órdenes de alquiler de prueba
            Console.WriteLine("🎯 Creating rental orders...");
            using (var scope5 = _serviceProvider.CreateScope())
            {
                var context = scope5.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var rentalOrderSeeder = new RentalOrderSeeder(context);
                await rentalOrderSeeder.SeedAsync();
            }

            Console.WriteLine("✅ Seeding process completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error during seeding: {ex.Message}");
            throw;
        }
    }
}
