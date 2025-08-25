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

            // 1. Roles y usuario administrador
            Console.WriteLine("📋 Creating roles and admin user...");
            var userDataSeeder = new UserDataSeeder(_serviceProvider);
            await userDataSeeder.SeedAsync();

            // 2. Géneros de libros
            Console.WriteLine("📚 Creating book genres...");
            var genreSeeder = new GenreSeeder(_serviceProvider);
            await genreSeeder.SeedAsync();

            // 3. Libros de muestra
            Console.WriteLine("📖 Creating sample books...");
            var bookSeeder = new BookSeeder(_serviceProvider);
            await bookSeeder.SeedAsync();

            // 4. Usuarios customer y entidades Customer
            Console.WriteLine("👥 Creating customer users...");
            var customerSeeder = new CustomerSeeder(_serviceProvider);
            await customerSeeder.SeedAsync();

            // 5. Órdenes de alquiler de prueba
            Console.WriteLine("🎯 Creating rental orders...");
            var rentalOrderSeeder = new RentalOrderSeeder(_serviceProvider);
            await rentalOrderSeeder.SeedAsync();

            Console.WriteLine("✅ Seeding process completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error during seeding: {ex.Message}");
            throw;
        }
    }
}
