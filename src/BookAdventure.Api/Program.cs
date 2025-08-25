using System.Text;
using BookAdventure.Api.Filters;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Persistence;
using BookAdventure.Persistence.Seeders;
using BookAdventure.Services.Profiles;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "log.txt");
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(logPath,
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

try
{
    builder.Logging.AddSerilog(logger);
    logger.Information($"LOG INITIALIZED in {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "NO ENV"}");
    //CORS
    var corsConfiguration = "MusicStoreCors";
    builder.Services.AddCors(setup =>
    {
        setup.AddPolicy(corsConfiguration, policy =>
        {
            policy.AllowAnyOrigin(); // Que cualquiera pueda consumir el API
            policy.AllowAnyHeader().WithExposedHeaders(new string[] { "TotalRecordsQuantity" });
            policy.AllowAnyMethod();
        });
    });

    builder.Services.AddControllers(options => { options.Filters.Add(typeof(FilterExceptions)); });

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = new BaseResponse
            {
                Success = false,
                ErrorMessage = string.Join("; ", errors) // Une los mensajes de error en un solo string.
            };

            return new BadRequestObjectResult(response);
        };
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configuring context
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    builder.Services.Configure<AppSettings>(builder.Configuration);

    // Registering the BookAdventure Repositories
    builder.Services.AddScoped<BookAdventure.Repositories.Interfaces.IBookRepository, BookAdventure.Repositories.Implementation.BookRepository>();
    builder.Services.AddScoped<BookAdventure.Repositories.Interfaces.IGenreRepository, BookAdventure.Repositories.Implementation.GenreRepository>();
    builder.Services.AddScoped<BookAdventure.Repositories.Interfaces.ICustomerRepository, BookAdventure.Repositories.Implementation.CustomerRepository>();
    builder.Services.AddScoped<BookAdventure.Repositories.Interfaces.IRentalOrderRepository, BookAdventure.Repositories.Implementation.RentalOrderRepository>();
    builder.Services.AddScoped<BookAdventure.Repositories.Interfaces.IRentalOrderDetailRepository, BookAdventure.Repositories.Implementation.RentalOrderDetailRepository>();

    // Registering the BookAdventure Services
    builder.Services.AddScoped<BookAdventure.Services.Interfaces.IBookService, BookAdventure.Services.Implementation.BookService>();
    builder.Services.AddScoped<BookAdventure.Services.Interfaces.IGenreService, BookAdventure.Services.Implementation.GenreService>();
    builder.Services.AddScoped<BookAdventure.Services.Interfaces.ICustomerService, BookAdventure.Services.Implementation.CustomerService>();
    builder.Services.AddScoped<BookAdventure.Services.Interfaces.IRentalOrderService, BookAdventure.Services.Implementation.RentalOrderService>();
    builder.Services.AddScoped<BookAdventure.Services.Interfaces.IRentalOrderDetailService, BookAdventure.Services.Implementation.RentalOrderDetailService>();
    builder.Services.AddScoped<BookAdventure.Services.Interfaces.IRentalQueryService, BookAdventure.Services.Implementation.RentalQueryService>();
    // TODO: Implement UserService and FileStorageLocal
    // builder.Services.AddScoped<BookAdventure.Services.Interfaces.IUserService, BookAdventure.Services.Implementation.UserService>();
    // builder.Services.AddScoped<BookAdventure.Services.Interfaces.IFileStorage, BookAdventure.Services.Implementation.FileStorageLocal>();

    // Registering Seeders
    builder.Services.AddScoped<UserDataSeeder>();
    builder.Services.AddScoped<GenreSeeder>();
    builder.Services.AddScoped<BookSeeder>();
    builder.Services.AddScoped<CustomerSeeder>();
    builder.Services.AddScoped<RentalOrderSeeder>();
    builder.Services.AddScoped<MasterSeeder>();
    builder.Services.AddHealthChecks()
        .AddCheck("selfcheck", () => HealthCheckResult.Healthy())
        .AddDbContextCheck<ApplicationDbContext>();

    // builder.Services.AddScoped<UserDataSeeder>();
    // builder.Services.AddScoped<GenreSeeder>();

    builder.Services.AddHttpContextAccessor();

    //Identity
    builder.Services.AddIdentity<BookAdventureUserIdentity, IdentityRole>(policies =>
        {
            policies.Password.RequireDigit = true;
            policies.Password.RequiredLength = 6;
            policies.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:JWTKey"] ??
                                         throw new InvalidOperationException("JWT key not configured"));
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
    builder.Services.AddAuthorization();

    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile<BookProfile>();
        config.AddProfile<GenreProfile>();
        config.AddProfile<CustomerProfile>();
        config.AddProfile<RentalOrderProfile>();
        config.AddProfile<RentalOrderDetailProfile>();
    });

    var app = builder.Build();

    // Applying migrations and seeding data
    await ApplyMigrationsAndSeedDataAsync(app);

    //if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCors(corsConfiguration);
    // app.MapReports();
    // app.MapHomeEndpoints();
    app.MapControllers();

    //Configuring health checks
    app.MapHealthChecks("/healthcheck", new()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "An unhandled exception occurred during the API initialization.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

static async Task ApplyMigrationsAndSeedDataAsync(WebApplication app)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Only apply migrations if we have pending ones and not running in migrations context
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            Console.WriteLine($"Applying {pendingMigrations.Count()} pending migrations...");
            await dbContext.Database.MigrateAsync();
        }

        // Only seed data if not running migrations command
        var args = Environment.GetCommandLineArgs();
        var isMigrationCommand = args.Any(arg => arg.Contains("migrations") || arg.Contains("database"));
        
        if (!isMigrationCommand)
        {
            var masterSeeder = scope.ServiceProvider.GetRequiredService<MasterSeeder>();
            await masterSeeder.SeedAllAsync();
            Console.WriteLine("Data seeding completed successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during migration/seeding: {ex.Message}");
        // Don't throw here to allow the app to continue
    }
}